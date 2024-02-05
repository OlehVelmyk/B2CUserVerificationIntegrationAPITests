using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Builders;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    [TestFixture]
    internal class ProfileTests : BaseComponentTest
    {
        private AdministratorFactory _adminFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private ApplicationFixture _applicationFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private EventsFixture _eventsFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _adminFactory = Resolve<AdministratorFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _eventsFixture = Resolve<EventsFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<TaxResidenceArbitrary>();
            Arb.Register<InvalidTaxResidenceArbitrary>();
            Arb.Register<TinArbitrary>();
            Arb.Register<InvalidTinArbitrary>();
            Arb.Register<IdDocumentNumberArbitrary>();
        }

        /// <summary>
        /// Scenario: Admin requests user profile by userId
        /// Given user with defined personal and verification details
        /// When admin requests user profile by userId
        /// Then he receives corresponding user profile
        /// </summary>
        [Theory]
        public async Task ShouldGetProfile(
            UserInfo userInfo,
            TaxResidence taxResidence,
            Tin tin,
            IdDocumentNumber idDocumentNumber,
            bool isPep,
            bool isSanctioned,
            bool isAdverseMedia)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(
                userId,
                builder => builder.With(taxResidence)
                                  .With(idDocumentNumber)
                                  .With(tin)
                                  .WithIsPep(isPep)
                                  .WithIsSanctioned(isSanctioned)
                                  .WithIsAdverseMedia(isAdverseMedia));

            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var profile = await client.Profile.GetAsync(userId);

            // Assert
            using (new AssertionScope())
            {
                profile.VerificationDetails.Tin.Should().BeEquivalentTo(tin);
                profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(taxResidence.Countries);
                profile.VerificationDetails.IdDocumentNumber.Should().BeEquivalentTo(idDocumentNumber);
                profile.VerificationDetails.VerificationIpAddress.Should().Be(userInfo.IpAddress);
                profile.VerificationDetails.IsPep.Should().Be(isPep);
                profile.VerificationDetails.IsSanctioned.Should().Be(isSanctioned);
                profile.VerificationDetails.IsAdverseMedia.Should().Be(isAdverseMedia);
            }
        }

        /// <summary>
        /// Scenario: Admin requests user profile by userId
        /// Given user without personal and verification details
        /// When admin requests user by userId
        /// Then he receives an error response with the status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenProfileDoNotExist(Guid userId)
        {
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            Func<Task> updateTaxResidence = async () => await client.Profile.GetAsync(userId);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Admin submits tax residence
        /// Given user with defined personal and verification details
        /// When admin submits tax residence
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTaxResidence(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(taxResidence.Countries);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var taxResidenceChange = changes.Find<string[]>(VerificationDetails.TaxResidence);
            taxResidenceChange.Should().NotBeNull();
            taxResidenceChange.PreviousValue.Should().BeNull();
            taxResidenceChange.NewValue.Should().BeEquivalentTo(taxResidence.Countries);
        }

        /// <summary>
        /// Scenario: Admin submits TIN
        /// Given user with defined personal and verification details
        /// When admin submits TIN
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTin(UserInfo userInfo, Tin tin)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(tin).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.Tin.Should().BeEquivalentTo(tin);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var tinChange = changes.Find<Tin>(VerificationDetails.Tin);
            tinChange.Should().NotBeNull();
            tinChange.PreviousValue.Should().BeNull();
            tinChange.NewValue.Should().BeEquivalentTo(tin);
        }

        /// <summary>
        /// Scenario: Admin submits IsAdverseMedia
        /// Given user with defined personal and verification details
        /// When admin submits IsAdverseMedia
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateIsAdverseMedia(UserInfo userInfo, bool isAdverseMedia)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsAdverseMedia(isAdverseMedia).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.IsAdverseMedia.Should().Be(isAdverseMedia);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var isAdverseMediaChange = changes.Find<bool?>(VerificationDetails.IsAdverseMedia);
            isAdverseMediaChange.Should().NotBeNull();
            isAdverseMediaChange.PreviousValue.Should().BeNull();
            isAdverseMediaChange.NewValue.Should().Be(isAdverseMedia);
        }

        /// <summary>
        /// Scenario: Admin submits IsPep
        /// Given user with defined personal and verification details
        /// When admin submits IsPep
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateIsPep(UserInfo userInfo, bool isPep)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsPep(isPep).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.IsPep.Should().Be(isPep);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var isPepChange = changes.Find<bool?>(VerificationDetails.IsPep);
            isPepChange.Should().NotBeNull();
            isPepChange.PreviousValue.Should().BeNull();
            isPepChange.NewValue.Should().Be(isPep);
        }

        /// <summary>
        /// Scenario: Admin submits IsSanctioned
        /// Given user with defined personal and verification details
        /// When admin submits IsSanctioned
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateIsSanctioned(UserInfo userInfo, bool isSanctioned)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsSanctioned(isSanctioned).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.IsSanctioned.Should().Be(isSanctioned);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var isSanctionedChange = changes.Find<bool?>(VerificationDetails.IsSanctioned);
            isSanctionedChange.Should().NotBeNull();
            isSanctionedChange.PreviousValue.Should().BeNull();
            isSanctionedChange.NewValue.Should().Be(isSanctioned);
        }

        /// <summary>
        /// Scenario: Admin submits IdDocumentNumber
        /// Given user with defined personal and verification details
        /// When admin submits IdDocumentNumber
        /// Then verification details updated
        /// And VerificationDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateIdDocumentNumber(UserInfo userInfo, IdDocumentNumber idDocumentNumber)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(idDocumentNumber).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.IdDocumentNumber.Should().BeEquivalentTo(idDocumentNumber);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var idDocumentNumberChange = changes.Find<IdDocumentNumber>(VerificationDetails.IdDocumentNumber);
            idDocumentNumberChange.Should().NotBeNull();
            idDocumentNumberChange.PreviousValue.Should().BeNull();
            idDocumentNumberChange.NewValue.Should().BeEquivalentTo(idDocumentNumber);
        }

        /// <summary>
        /// Scenario: Admin submits tax residences
        /// Given user with defined personal and verification details
        /// When admin submits invalid tax residence
        /// Then receive error response with status code "Bad Request"
        /// And event is not raised
        /// </summary>
        [Theory, Ignore("Admin api do not have validation")]
        public async Task ShouldGetError_WhenSubmitInvalidTaxResidence(UserInfo userInfo, InvalidTaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request, userId);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.TaxResidence.Should().BeNullOrEmpty();

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits invalid TIN
        /// And user with defined personal and verification details
        /// When submit invalid TIN
        /// Then receive error response with status code "Bad Request"
        /// And event is not raised
        /// </summary>
        [Theory, Ignore("Admin api do not have validation")]
        public async Task ShouldGetError_WhenSubmitInvalidTin(UserInfo userInfo, InvalidTin tin)
        {
            // Given
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(tin).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request, userId);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var profile = await client.Profile.GetAsync(userId);
            profile.VerificationDetails.Tin.Should().BeNull();

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits tax residence
        /// Given user with defined personal and verification details
        /// When admin submit tax residence
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateTaxResidence_WhenSubmitSameValue(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(taxResidence));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits TIN
        /// Given user with defined personal and verification details
        /// When admin submits TIN
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateTin_WhenSubmitSameValue(UserInfo userInfo, Tin tin)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(tin));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(tin).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits IsPep
        /// Given user with defined personal and verification details
        /// When Admin submits IsPep
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateIsPep_WhenSubmitSameValue(UserInfo userInfo, bool isPep)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsPep(isPep));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsPep(isPep).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits IsSanctioned
        /// Given user with defined personal and verification details
        /// When admin submits IsSanctioned
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateIsSanctioned_WhenSubmitSameValue(UserInfo userInfo, bool isSanctioned)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsSanctioned(isSanctioned));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsSanctioned(isSanctioned).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits IsAdverseMedia
        /// Given user who has personal and verification details
        /// When admin submits IsAdverseMedia
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateIsAdverseMedia_WhenSubmitSameValue(UserInfo userInfo, bool isAdverseMedia)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsAdverseMedia(isAdverseMedia));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().WithIsAdverseMedia(isAdverseMedia).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Admin submits IdDocumentNumber by same value
        /// Given user with defined personal and verification details
        /// When admin submit same IdDocumentNumber
        /// And value is the same
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateIdDocumentNumber_WhenSubmitSameValue(UserInfo userInfo, IdDocumentNumber idDocumentNumber)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(idDocumentNumber));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var request = new AdminUpdateVerificationDetailsRequestBuilder().With(idDocumentNumber).Build();

            // Act
            await client.Profile.UpdateAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }
    }
}
