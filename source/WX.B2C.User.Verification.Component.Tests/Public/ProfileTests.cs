using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
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
using WX.B2C.User.Verification.Extensions;
using ApplicationState = WX.B2C.User.Verification.Api.Public.Client.Models.ApplicationState;
using ErrorResponseException = WX.B2C.User.Verification.Api.Public.Client.Models.ErrorResponseException;
using TinType = WX.B2C.User.Verification.Api.Public.Client.Models.TinType;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    [TestFixture]
    internal class ProfileTests : BaseComponentTest
    {
        private PublicApiClientFactory _publicApiClientFactory;
        private ApplicationFixture _applicationFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private VerificationDetailsStepFixture _verificationDetailsStepFixture;
        private CollectionStepsFixture _collectionStepsFixture;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _verificationDetailsStepFixture = Resolve<VerificationDetailsStepFixture>();
            _collectionStepsFixture = Resolve<CollectionStepsFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<TinArbitrary>();
            Arb.Register<InvalidTinArbitrary>();
            Arb.Register<TaxResidenceArbitrary>();
            Arb.Register<InvalidTaxResidenceArbitrary>();
            Arb.Register<IdDocumentNumberArbitrary>();
        }

        /// <summary>
        /// Scenario: User gets his profile info
        /// Given user with verification details
        /// And with applied application
        /// When user requests profile
        /// Then he receives his profile information
        /// And profile is fully completed
        /// </summary>
        [Theory]
        public async Task ShouldGetProfile(UsUserInfo userInfo, TaxResidence taxResidence, Tin tin, IdDocumentNumber idDocumentNumber)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByUserAsync(userId, builder => builder.With(tin).With(taxResidence));
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(idDocumentNumber));

            // Arrange
            var client = _publicApiClientFactory.Create(userId);

            // Act
            var profile = await client.Profile.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                profile.ApplicationState.Should().Be(ApplicationState.Applied);
                profile.VerificationDetails.Tin.Should().BeEquivalentTo(tin);
                profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(taxResidence.Countries);
                profile.VerificationDetails.IdDocumentNumber.Should().BeEquivalentTo(idDocumentNumber);
            }
        }

        /// <summary>
        /// Scenario: User gets his profile info
        /// Given user with verification details without TIN
        /// When user requests profile
        /// Then he receives his profile information
        /// And TIN is null in profile
        /// And all other properties are present
        /// </summary>
        [Theory]
        public async Task ShouldGetPartiallyCompletedProfile(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsFixture.UpdateByUserAsync(userId, builder => builder.With(taxResidence));

            // Arrange
            var client = _publicApiClientFactory.Create(userId);

            // Act
            var profile = await client.Profile.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                profile.ApplicationState.Should().Be(ApplicationState.Applied);
                profile.VerificationDetails.Tin.Should().BeNull();
                profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(taxResidence.Countries);
                profile.VerificationDetails.IdDocumentNumber.Should().BeNull();
            }
        }

        /// <summary>
        /// Scenario: User gets his profile info
        /// Given user with personal details
        /// And without verification details
        /// And without applied application
        /// When user requests profile
        /// Then he receives his profile information
        /// And all fields is null
        /// </summary>
        [Theory]
        public async Task ShouldGetNull_WhenProfileEmpty(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);

            // Act
            var profile = await client.Profile.GetAsync();

            // Assert
            profile.ApplicationState.Should().BeNull();
            profile.VerificationDetails.Should().BeNull();
        }

        /// <summary>
        /// Scenario: User gets his profile info
        /// Given user with profile
        /// And without applied application
        /// When user requests profile
        /// Then he receives profile
        /// And application state is null
        /// </summary>
        [Theory]
        public async Task ShouldGetNull_WhenUserDoNotStartVerification(Guid userId, TaxResidence taxResidence)
        {
            // Given
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(taxResidence));

            // Arrange
            var client = _publicApiClientFactory.Create(userId);

            // Act
            var profile = await client.Profile.GetAsync();

            // Assert
            profile.ApplicationState.Should().BeNull();
        }

        /// <summary>
        /// Scenario: User submits valid tax residence
        /// Given user with defined personal and verification details
        /// And tax residence is not submitted yet
        /// And tax residence collection step is requested
        /// When user submits valid tax residence
        /// Then verification details should be updated
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSubmitTaxResidence(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            var profile = await client.Profile.GetAsync();
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
        /// Scenario: User submits valid TIN
        /// Given user with defined personal and verification details
        /// And TIN is not submitted yet
        /// And TIN collection step is requested
        /// When user submits valid TIN
        /// Then verification details should be updated
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSubmitTin(UsUserInfo userInfo, Tin tin)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId:correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(tin).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.Tin.Number.Should().Be(tin.Number);
            profile.VerificationDetails.Tin.Type.Should().Be(tin.Type.To<TinType>());

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var tinChange = changes.Find<Tin>(VerificationDetails.Tin);
            tinChange.Should().NotBeNull();
            tinChange.PreviousValue.Should().BeNull();
            tinChange.NewValue.Should().BeEquivalentTo(tin);
        }

        /// <summary>
        /// Scenario: User updates tax residence
        /// Given user with defined personal and verification details
        /// And tax residence is already submitted
        /// And tax residence collection step is requested
        /// When submit new tax residence
        /// Then verification details should be updated
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTaxResidence_WhenAdminRequestIt(UserInfo userInfo, TaxResidence initial, TaxResidence newValue)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, initial);
            await _collectionStepsFixture.RequestAsync(userId, VerificationDetailsProperty.TaxResidence);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(newValue).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(newValue.Countries);

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var taxResidenceChange = changes.Find<string[]>(VerificationDetails.TaxResidence);
            taxResidenceChange.Should().NotBeNull();
            taxResidenceChange.PreviousValue.Should().BeEquivalentTo(initial.Countries);
            taxResidenceChange.NewValue.Should().BeEquivalentTo(newValue.Countries);
        }

        /// <summary>
        /// Scenario: User updates TIN
        /// Given user with defined personal and verification details
        /// And TIN is already submitted
        /// And TIN collection step is requested
        /// When submit new TIN
        /// Then verification details should be updated
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTin_WhenAdminRequestIt(UsUserInfo userInfo, Tin initial, Tin newValue)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, initial);
            await _collectionStepsFixture.RequestAsync(userId, VerificationDetailsProperty.Tin);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(newValue).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.Tin.Number.Should().Be(newValue.Number);
            profile.VerificationDetails.Tin.Type.Should().Be(newValue.Type.To<TinType>());

            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            changes.Should().HaveCount(1);
            var tinChange = changes.Find<Tin>(VerificationDetails.Tin);
            tinChange.Should().NotBeNull();
            tinChange.PreviousValue.Should().BeEquivalentTo(initial);
            tinChange.NewValue.Should().BeEquivalentTo(newValue);
        }

        /// <summary>
        /// Scenario: User updates tax residence
        /// Given user with defined personal and verification details
        /// And tax residence is already submitted
        /// And tax residence collection step is not requested
        /// When submit new tax residence
        /// Then receive error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenTaxResidenceIsNotRequested(UserInfo userInfo, TaxResidence initial, TaxResidence newValue)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, initial);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(newValue).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.TaxResidence.Should().BeEquivalentTo(initial.Countries);

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User updates TIN
        /// Given user with defined personal and verification details
        /// And TIN is already submitted
        /// And TIN collection step is not requested
        /// When submit new TIN
        /// Then receive error response with status code "Bad Request"
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenTinIsNotRequested(UsUserInfo userInfo, Tin initial, Tin newValue)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, initial);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId:correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(newValue).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User updates tax residence by same value
        /// Given user with defined personal and verification details
        /// And tax residence is already submitted
        /// And tax residence collection step is requested
        /// When submit new tax residence same to given
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateTaxResidence_WhenSubmitSameValue(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, taxResidence);
            await _collectionStepsFixture.RequestAsync(userId, VerificationDetailsProperty.TaxResidence);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User updates TIN by same value
        /// Given user with defined personal and verification details
        /// And TIN is already submitted
        /// And TIN collection step is requested
        /// When submit new TIN same to given
        /// Then event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateTin_WhenSubmitSameValue(UsUserInfo userInfo, Tin tin)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _verificationDetailsStepFixture.CompleteAsync(userId, tin);
            await _collectionStepsFixture.RequestAsync(userId, VerificationDetailsProperty.Tin);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, correlationId:correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(tin).Build();

            // Act
            await client.Profile.UpdateAsync(request);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User submits invalid tax residence
        /// Given user with defined personal and verification details
        /// And tax residence collection step is requested
        /// When users submits tax residence
        /// And value does not satisfy validation rules
        /// Then receive error response with status code "Bad Request"
        /// And event is not raised
        /// </summary>
        [Theory]
        // TODO: Flaky until WRXB-10534
        public async Task ShouldGetError_WhenSubmitInvalidTaxResidence(UserInfo userInfo, InvalidTaxResidence taxResidence)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId:correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(taxResidence).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.TaxResidence.Should().BeNull();

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User submits invalid TIN
        /// Given user with defined personal and verification details
        /// And TIN collection step is requested
        /// When user submits TIN
        /// And value does not satisfy validation rules
        /// Then receive error response with status code "Bad Request"
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenSubmitInvalidTin(UsUserInfo userInfo, InvalidTin tin)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var request = new UserUpdateVerificationDetailsRequestBuilder().With(tin).Build();
            Func<Task> updateTaxResidence = async () => await client.Profile.UpdateAsync(request);

            // Act & Assert
            var errorResponse = await updateTaxResidence.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var profile = await client.Profile.GetAsync();
            profile.VerificationDetails.Tin.Should().BeNull();

            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(correlationId);
        }
    }
}
