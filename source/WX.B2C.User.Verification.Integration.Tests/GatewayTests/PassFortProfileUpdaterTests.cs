using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using Microsoft.Rest;
using NUnit.Framework;
using Optional;
using Serilog.Core;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Configuration.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.PassFort;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.IoC;
using WX.B2C.User.Verification.PassFort.Models;
using WX.Configuration.Contracts;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    public class PassFortProfileUpdaterTests : BaseIntegrationTest
    {
        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSystemClock();
            containerBuilder.RegisterConfiguration();
            containerBuilder.RegisterBlobStorage();
            containerBuilder.RegisterConfigurations();
            containerBuilder.RegisterPassFortGateway();

            containerBuilder.Register(context => new SelfHostingValuesResolver("PassFort", "KeyVault"))
                            .As<IHostingSpecificValuesResolver>();
            containerBuilder.Register(context =>
                            {
                                var valueResolver = context.Resolve<IHostingSpecificValuesResolver>();
                                var settings = new PassFortApiSettings
                                {
                                    ApiKey = valueResolver.GetValue("ApiKey"),
                                    ApiUri = new Uri(valueResolver.GetValue("ApiUri"))
                                };

                                var clientFactory = new PassFortApiClientFactory(settings, new PassFortPolicyFactory(Logger.None));

                                return clientFactory;
                            })
                            .As<IPassFortApiClientFactory>();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Arb.Register<PassFortProfileArbitrary>();
            Arb.Register<ResidenceAddressArbitrary>();
            Arb.Register<PersonalDetailsArbitrary>();
        }

        [Theory]
        public async Task ShouldUpdateProfile(PassFortProfile profile, PersonalDetailsDto personalDetails, AddressDto address)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();
            var countryDetailsProvider = Resolve<ICountryDetailsProvider>();
            var expectedNationality = await countryDetailsProvider.FindAlpha3Async(personalDetails.Nationality);
            var expectedCountryCode = await countryDetailsProvider.FindAlpha3Async(address.Country);

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            // Arrange
            var passFortProfilePatch = new PassFortProfilePatch
            {
                Email = personalDetails.Email.Some(),
                BirthDate = personalDetails.DateOfBirth.ToOption(),
                FullName = new FullNameDto { FirstName = personalDetails.FirstName, LastName = personalDetails.LastName }.Some(),
                Nationality = personalDetails.Nationality.SomeNotNull(),
                IdDocumentData = ("FRA", new IdDocumentNumberDto { Type = "passport", Number = "8888888" }).Some(),
                Address = address.Some()
            };

            // Act
            await updater.UpdateAsync(profileId, passFortProfilePatch);

            // Arrange
            var updatedProfile = await client.Profiles.GetAsync(profileId);

            var collectedData = updatedProfile.CollectedData;
            collectedData.PersonalDetails.Nationality.Should().Be(expectedNationality);
            collectedData.PersonalDetails.Name.FamilyName.Should().Be(personalDetails.LastName);
            collectedData.PersonalDetails.Name.GivenNames.Should().ContainSingle(personalDetails.FirstName);
            collectedData.PersonalDetails.Dob.Should()
                         .Be(personalDetails.DateOfBirth.HasValue ? personalDetails.DateOfBirth.Value.ToString("yyyy-MM-dd") : null);
            collectedData.AddressHistory.Should().HaveCount(1);
            var actualAddress = collectedData.AddressHistory.First().Address as StructuredAddress;
            actualAddress.Should().NotBeNull();
            actualAddress.Country.Should().Be(expectedCountryCode);
            actualAddress.OriginalFreeformAddress.Should().ContainAll(address.City, address.Line1, address.State, address.ZipCode);
        }

        [Theory]
        public async Task ShouldNotUpdateProfileSecondTime(PassFortProfile profile, PersonalDetailsDto personalDetails, AddressDto address)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            var passFortProfilePatch = new PassFortProfilePatch
            {
                Email = personalDetails.Email.Some(),
                BirthDate = personalDetails.DateOfBirth.ToOption(),
                FullName = new FullNameDto { FirstName = personalDetails.FirstName, LastName = personalDetails.LastName }.Some(),
                Nationality = personalDetails.Nationality.SomeNotNull(),
                IdDocumentData = ("FRA", new IdDocumentNumberDto { Type = "passport", Number = "8888888" }).Some(),
                Address = address.Some()
            };
            await updater.UpdateAsync(profileId, passFortProfilePatch);

            // Arrange
            var passFortClientInterceptor = new PassFortClientInterceptor();
            ServiceClientTracing.AddTracingInterceptor(passFortClientInterceptor);

            // Act
            ServiceClientTracing.IsEnabled = true;
            await updater.UpdateAsync(profileId, passFortProfilePatch);
            ServiceClientTracing.IsEnabled = false;

            // Arrange
            passFortClientInterceptor.UpdateCollectedDataRequests.Count.Should().Be(0);
        }

        [Theory]
        public async Task ShouldNotUpdateProfileSecondTime_WhenIdDocNumberIsFromPredefinedCountries(PassFortProfile profile)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            var passFortProfilePatch = new PassFortProfilePatch
            {
                Nationality = "SE".SomeNotNull(),
                IdDocumentData = ("FRA", new IdDocumentNumberDto { Type = "passport", Number = "8888888" }).Some(),
            };
            await updater.UpdateAsync(profileId, passFortProfilePatch);

            // Arrange
            var passFortClientInterceptor = new PassFortClientInterceptor();
            ServiceClientTracing.AddTracingInterceptor(passFortClientInterceptor);

            // Act
            ServiceClientTracing.IsEnabled = true;
            await updater.UpdateAsync(profileId, passFortProfilePatch);
            ServiceClientTracing.IsEnabled = false;

            // Arrange
            passFortClientInterceptor.UpdateCollectedDataRequests.Count.Should().Be(0);
        }

        [Theory]
        public async Task ShouldNotUpdateProfile(PassFortProfile profile, PersonalDetailsDto personalDetails, AddressDto address)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            var passFortProfilePatch = new PassFortProfilePatch
            {
                Email = personalDetails.Email.Some(),
                BirthDate = personalDetails.DateOfBirth.ToOption(),
                FullName = new FullNameDto { FirstName = personalDetails.FirstName, LastName = personalDetails.LastName }.Some(),
                Nationality = personalDetails.Nationality.SomeNotNull(),
                IdDocumentData = ("FRA", new IdDocumentNumberDto { Type = "passport", Number = "8888888" }).Some(),
                Address = address.Some()
            };
            await updater.UpdateAsync(profileId, passFortProfilePatch);

            // Arrange
            address.City = "New " + address.City;
            passFortProfilePatch.Address = address.Some();

            var passFortClientInterceptor = new PassFortClientInterceptor();
            ServiceClientTracing.AddTracingInterceptor(passFortClientInterceptor);

            // Act
            ServiceClientTracing.IsEnabled = true;
            await updater.UpdateAsync(profileId, passFortProfilePatch);
            ServiceClientTracing.IsEnabled = false;

            // Arrange
            passFortClientInterceptor.UpdateCollectedDataRequests.Count.Should().Be(1);
            var updatedProfile = await client.Profiles.GetAsync(profileId);
            var structuredAddress = updatedProfile.CollectedData.AddressHistory?.FirstOrDefault()?.Address as StructuredAddress;
            structuredAddress.Should().NotBeNull();
            structuredAddress.OriginalFreeformAddress.Should().ContainAll(address.City, address.Line1, address.State, address.ZipCode);
        }

        [Theory]
        public async Task ShouldUpdateProfileOnlyOnce_WhenUpdatingInParallel(PassFortProfile profile,
                                                                             PersonalDetailsDto personalDetails,
                                                                             AddressDto address)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            var passFortProfilePatch = new PassFortProfilePatch
            {
                Email = personalDetails.Email.Some(),
                BirthDate = personalDetails.DateOfBirth.ToOption(),
                FullName = new FullNameDto { FirstName = personalDetails.FirstName, LastName = personalDetails.LastName }.Some(),
                Nationality = personalDetails.Nationality.SomeNotNull(),
                IdDocumentData = ("FRA", new IdDocumentNumberDto { Type = "passport", Number = "8888888" }).Some(),
                Address = address.Some()
            };

            // Arrange
            var passFortClientInterceptor = new PassFortClientInterceptor();
            ServiceClientTracing.AddTracingInterceptor(passFortClientInterceptor);

            // Act
            ServiceClientTracing.IsEnabled = true;
            var parallelAttemptsNumber = 10;
            var tasks = new Task[parallelAttemptsNumber];
            for (int i = 0; i < parallelAttemptsNumber; i++)
            {
                tasks[i] = updater.UpdateAsync(profileId, passFortProfilePatch);
            }
            await Task.WhenAll(tasks);
            ServiceClientTracing.IsEnabled = false;

            // Arrange

            // Each thread can try to update one time.
            passFortClientInterceptor.UpdateCollectedDataRequests.Count.Should().BeLessOrEqualTo(parallelAttemptsNumber);

            passFortClientInterceptor.UpdateCollectedDataResponses.Where(message => message.Response.IsSuccessStatusCode)
                                     .Should()
                                     .HaveCount(1);
        }

        [Theory]
        public async Task ShouldUpdateProfile_WhenUpdatingInParallelWithDifferentPatches(
            PassFortProfile profile,
            PersonalDetailsDto personalDetails,
            AddressDto address)
        {
            // Given
            var client = Resolve<IPassFortApiClientFactory>().Create();
            var updater = Resolve<IPassFortProfileUpdater>();
            var countryDetailsProvider = Resolve<ICountryDetailsProvider>();
            var expectedNationality = await countryDetailsProvider.FindAlpha3Async(personalDetails.Nationality);
            var expectedCountryCode = await countryDetailsProvider.FindAlpha3Async(address.Country);

            var createdProfile = await client.Profiles.CreateAsync(profile);
            var profileId = createdProfile.Id;

            // Arrange
            var patches = new List<PassFortProfilePatch>
            {
                new()
                {
                    Email = personalDetails.Email.Some(),
                },
                new()
                {
                    BirthDate = personalDetails.DateOfBirth.ToOption(),
                },
                new()
                {
                    FullName = new FullNameDto { FirstName = personalDetails.FirstName, LastName = personalDetails.LastName }.Some(),
                },
                new()
                {
                    Nationality = personalDetails.Nationality.SomeNotNull(),
                },
                new()
                {
                    Address = address.Some()
                }
            };

            var passFortClientInterceptor = new PassFortClientInterceptor();
            ServiceClientTracing.AddTracingInterceptor(passFortClientInterceptor);

            // Act
            ServiceClientTracing.IsEnabled = true;
            await patches.Foreach(patch => updater.UpdateAsync(profileId, patch));
            ServiceClientTracing.IsEnabled = false;

            // Arrange
            // Each thread can try to update one time.
            passFortClientInterceptor.UpdateCollectedDataResponses.Where(message => message.Response.IsSuccessStatusCode)
                                     .Should()
                                     .HaveCount(patches.Count);

            var updatedProfile = await client.Profiles.GetAsync(profileId);

            var collectedData = updatedProfile.CollectedData;
            collectedData.PersonalDetails.Nationality.Should().Be(expectedNationality);
            collectedData.PersonalDetails.Name.FamilyName.Should().Be(personalDetails.LastName);
            collectedData.PersonalDetails.Name.GivenNames.Should().ContainSingle(personalDetails.FirstName);
            collectedData.PersonalDetails.Dob.Should()
                         .Be(personalDetails.DateOfBirth.HasValue ? personalDetails.DateOfBirth.Value.ToString("yyyy-MM-dd") : null);
            collectedData.AddressHistory.Should().HaveCount(1);
            var actualAddress = collectedData.AddressHistory.First().Address as StructuredAddress;
            actualAddress.Should().NotBeNull();
            actualAddress.Country.Should().Be(expectedCountryCode);
            actualAddress.OriginalFreeformAddress.Should().ContainAll(address.City, address.Line1, address.State, address.ZipCode);
        }
    }
}