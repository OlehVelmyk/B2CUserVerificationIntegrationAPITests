using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.Storages;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Provider.Services.IoC;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class DuplicateSearchServiceTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IDuplicateSearchService _sut;
        private IPersonalDetailsRepository _personalDetailsRepository;
        private IVerificationDetailsRepository _verificationDetailsRepository;
        private IOptionsProvider _optionsProvider;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            _optionsProvider = Substitute.For<IOptionsProvider>();
            var hostSettingsProvider = Substitute.For<IHostSettingsProvider>();
            hostSettingsProvider.GetSetting(Arg.Any<string>()).Returns("Local");

            containerBuilder.RegisterInstance(hostSettingsProvider).SingleInstance();
            containerBuilder.RegisterInstance(_optionsProvider).SingleInstance();
            containerBuilder.RegisterType<DuplicateSearchService>().SingleInstance();
            containerBuilder.RegisterSandboxDecorators();
        }

        [SetUp]
        public void SetUp()
        {
            var excludedNameOption = new ExcludedNameOption(string.Empty, string.Empty);
            var option = new ExcludedNamesOption(new[] { excludedNameOption });
            _optionsProvider.GetAsync<ExcludedNamesOption>().Returns(option);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IDuplicateSearchService>();
            _personalDetailsRepository = Resolve<IPersonalDetailsRepository>();
            _verificationDetailsRepository = Resolve<IVerificationDetailsRepository>();

            Arb.Register<PersonalDetailsArbitrary>();
            Arb.Register<VerificationDetailsArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldFindDuplicatedMatches((PersonalDetailsDto, PersonalDetailsDto) personaldetails, DateTime dob)
        {
            var (first, second) = personaldetails;
            first.DateOfBirth = second.DateOfBirth = dob;
            (first.FirstName, first.LastName) = (second.FirstName, second.LastName);

            _users.Add(first.UserId);
            _users.Add(second.UserId);

            await _personalDetailsRepository.SaveAsync(first);
            await _personalDetailsRepository.SaveAsync(second);

            // Arrange
            var fullName = new FullNameDto
            {
                FirstName = first.FirstName,
                LastName = first.LastName
            };
            var context = DuplicateSearchContext.Create(first.UserId, fullName, first.DateOfBirth.Value);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().NotBeEmpty();
            result.Total.Should().Be(1);
        }

        [Theory]
        public async Task ShouldNotFindDuplicatedMatches(PersonalDetailsDto personaldetails, DateTime dob)
        {
            personaldetails.DateOfBirth = dob;
            _users.Add(personaldetails.UserId);
            await _personalDetailsRepository.SaveAsync(personaldetails);

            // Arrange
            var fullName = new FullNameDto
            {
                FirstName = personaldetails.FirstName,
                LastName = personaldetails.LastName
            };
            var context = DuplicateSearchContext.Create(personaldetails.UserId, fullName, personaldetails.DateOfBirth.Value);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().BeEmpty();
            result.Total.Should().Be(0);
        }

        [Theory]
        public async Task ShouldNotFindDuplicatedMatches_WhenNamesExcluded((PersonalDetailsDto, PersonalDetailsDto) personaldetails, DateTime dob)
        {
            var (first, second) = personaldetails;
            first.DateOfBirth = second.DateOfBirth = dob;
            (first.FirstName, first.LastName) = (second.FirstName, second.LastName);

            await _personalDetailsRepository.SaveAsync(first);
            await _personalDetailsRepository.SaveAsync(second);

            _users.Add(first.UserId);
            _users.Add(second.UserId);

            var excludedNameOption = new ExcludedNameOption(first.FirstName, first.LastName);
            var option = new ExcludedNamesOption(new[] { excludedNameOption });
            _optionsProvider.GetAsync<ExcludedNamesOption>().Returns(option);

            // Arrange
            var fullName = new FullNameDto
            {
                FirstName = first.FirstName,
                LastName = first.LastName
            };
            var context = DuplicateSearchContext.Create(first.UserId, fullName, first.DateOfBirth.Value);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().BeEmpty();
            result.Total.Should().Be(0);
        }

        [Theory]
        public async Task ShouldNotFindDuplicatedMatches_WhenOnlyFirstNameExcluded((PersonalDetailsDto, PersonalDetailsDto, PersonalDetailsDto) personaldetails, DateTime dob)
        {
            var (first, second, third) = personaldetails;
            first.DateOfBirth = second.DateOfBirth = third.DateOfBirth = dob;
            (first.FirstName, first.LastName) = (second.FirstName, _) = (third.FirstName, third.LastName);

            await _personalDetailsRepository.SaveAsync(first);
            await _personalDetailsRepository.SaveAsync(second);
            await _personalDetailsRepository.SaveAsync(third);

            _users.Add(first.UserId);
            _users.Add(second.UserId);
            _users.Add(third.UserId);

            var excludedNameOption = new ExcludedNameOption(first.FirstName, ExcludedNameOption.AnyNameKey);
            var option = new ExcludedNamesOption(new[] { excludedNameOption });
            _optionsProvider.GetAsync<ExcludedNamesOption>().Returns(option);

            // Arrange
            var fullName = new FullNameDto
            {
                FirstName = first.FirstName,
                LastName = first.LastName
            };
            var context = DuplicateSearchContext.Create(first.UserId, fullName, first.DateOfBirth.Value);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().BeEmpty();
            result.Total.Should().Be(0);
        }

        [Theory]
        public async Task ShouldFindDuplicatedMatches_WhenNoDecorator((PersonalDetailsDto, PersonalDetailsDto) personaldetails, DateTime dob)
        {
            var (first, second) = personaldetails;
            first.DateOfBirth = second.DateOfBirth = dob;
            (first.FirstName, first.LastName) = (second.FirstName, second.LastName);

            await _personalDetailsRepository.SaveAsync(first);
            await _personalDetailsRepository.SaveAsync(second);

            _users.Add(first.UserId);
            _users.Add(second.UserId);

            var excludedNameOption = new ExcludedNameOption(first.FirstName, first.LastName);
            var option = new ExcludedNamesOption(new[] { excludedNameOption });
            _optionsProvider.GetAsync<ExcludedNamesOption>().Returns(option);

            // Arrange
            var fullName = new FullNameDto
            {
                FirstName = first.FirstName,
                LastName = first.LastName
            };
            var context = DuplicateSearchContext.Create(first.UserId, fullName, first.DateOfBirth.Value);

            // Act
            var sut = Resolve<DuplicateSearchService>();
            var result = await sut.FindAsync(context);

            // Assert
            result.Matches.Should().NotBeEmpty();
            result.Total.Should().Be(1);
        }

        [Theory]
        public async Task ShouldFindDuplicatedMatches_WhenIdDocNumber((VerificationDetailsDto, VerificationDetailsDto) verificationDetails)
        {
            var (first, second) = verificationDetails;
            first.IdDocumentNumber = second.IdDocumentNumber;

            _users.Add(first.UserId);
            _users.Add(second.UserId);

            await _verificationDetailsRepository.SaveAsync(first);
            await _verificationDetailsRepository.SaveAsync(second);

            // Arrange
            var context = DuplicateSearchContext.Create(first.UserId, first.IdDocumentNumber);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().NotBeEmpty();
            result.Total.Should().Be(1);
        }

        [Theory]
        public async Task ShouldNotFindDuplicatedMatches_WhenIdDocNumber(VerificationDetailsDto verificationDetails)
        {
            _users.Add(verificationDetails.UserId);
            await _verificationDetailsRepository.SaveAsync(verificationDetails);

            // Arrange
            var context = DuplicateSearchContext.Create(verificationDetails.UserId, verificationDetails.IdDocumentNumber);

            // Act
            var result = await _sut.FindAsync(context);

            // Assert
            result.Matches.Should().BeEmpty();
            result.Total.Should().Be(0);
        }
    }
}
