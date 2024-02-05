using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Profile.Events.EventArgs;
using WX.B2C.User.Profile.Events.Events;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Builders;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Events
{
    internal class UserProfileEventsTests : BaseComponentTest
    {
        private AdministratorFactory _adminFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _adminFactory = Resolve<AdministratorFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<ApacUserInfoArbitrary>();
            Arb.Register<EeaUserInfoArbitrary>();
            Arb.Register<PersonalDetailsPatchArbitrary>();
            Arb.Register<InvalidAddressArbitrary>();
        }

        /// <summary>
        /// Scenario: UserProfileCreated event is raised
        /// Given user without personal details
        /// When UserProfileCreated event is raised
        /// Then personal details is created
        /// And PersonalDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreatePersonalDetails_WhenUserProfileCreatedEventRaised(UserInfo userInfo, PersonalDetailsPatch patch)
        {
            // Arrange
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var profileCreatedEvent = new ProfileEventsBuilder(correlationId).With(userInfo, patch).BuildProfileCreatedEvent();

            // Act
            await _eventsFixture.PublishAsync(profileCreatedEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<PersonalDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            var personalDetails = (await client.Profile.GetAsync(userId)).PersonalDetails;
            foreach (var property in patch.Properties)
            {
                var expectedValue = property.GetExpectedValue(userInfo, personalDetails);
                var actualValue = property.GetActualValue(personalDetails);
                actualValue.Should().BeEquivalentTo(expectedValue);

                var change = property.GetChange(changes);
                change.Should().NotBeNull($"PropertyChange for {property.Type} was not found");
                change.PreviousValue.Should().BeNull();
                change.NewValue.Should().BeEquivalentTo(expectedValue);
            }
        }

        /// <summary>
        /// Scenario: UserProfileUpdated event is raised
        /// Given user without personal details
        /// When UserProfileUpdated event is raised
        /// Then personal details is created
        /// And PersonalDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreatePersonalDetails(UserInfo userInfo, PersonalDetailsPatch patch)
        {
            // Arrange
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var profileUpdatedEvent = new ProfileEventsBuilder(correlationId).With(userInfo, patch).BuildProfileUpdatedEvent();

            // Act
            await _eventsFixture.PublishAsync(profileUpdatedEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<PersonalDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            var personalDetails = (await client.Profile.GetAsync(userId)).PersonalDetails;
            foreach (var property in patch.Properties)
            {
                var expectedValue = property.GetExpectedValue(userInfo, personalDetails);
                var actualValue = property.GetActualValue(personalDetails);
                actualValue.Should().BeEquivalentTo(expectedValue);

                var change = property.GetChange(changes);
                change.Should().NotBeNull($"PropertyChange for {property.Type} was not found");
                change.PreviousValue.Should().BeNull();
                change.NewValue.Should().BeEquivalentTo(expectedValue);
            }
        }

        /// <summary>
        /// Scenario: UserProfileUpdated event is raised
        /// Given user with personal details
        /// When UserProfileUpdated event is raised
        /// Then personal details is updated
        /// And PersonalDetailsUpdated event is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdatePersonalDetails(ApacUserInfo initial, EeaUserInfo newValue, PersonalDetailsPatch patch)
        {
            // Given
            var userId = initial.UserId.Dump();
            await _profileFixture.CreateAsync(initial);

            // Arrange
            newValue.UserId = userId;
            var correlationId = Guid.NewGuid();
            var profileUpdatedEvent = new ProfileEventsBuilder(correlationId).With(newValue, patch).BuildProfileUpdatedEvent();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            await _eventsFixture.PublishAsync(profileUpdatedEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<PersonalDetailsUpdatedEvent>(correlationId);
            var changes = @event.EventArgs.Changes;
            var personalDetails = (await client.Profile.GetAsync(userId)).PersonalDetails;
            foreach (var property in patch.Properties)
            {
                var expectedValue = property.GetExpectedValue(newValue, personalDetails);
                var actualValue = property.GetActualValue(personalDetails);
                actualValue.Should().BeEquivalentTo(expectedValue);

                var change = property.GetChange(changes);
                change.Should().NotBeNull($"PropertyChange for {property.Type} was not found");
                change.PreviousValue.Should().BeEquivalentTo(property.GetExpectedValue(initial, personalDetails));
                change.NewValue.Should().BeEquivalentTo(expectedValue);
            }
        }

        /// <summary>
        /// Scenario: UserProfileUpdated event is raised
        /// Given user with personal details
        /// When UserProfileUpdated event is raised
        /// And it has same data as personal details
        /// Then personal details became unchanged
        /// And PersonalDetailsUpdated event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdate_WhenDataIsSame(UserInfo userInfo, PersonalDetailsPatch patch)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var profileUpdatedEvent = new ProfileEventsBuilder(correlationId).With(userInfo, patch).BuildProfileUpdatedEvent();

            // Act
            await _eventsFixture.PublishAsync(profileUpdatedEvent);

            // Assert
            _eventsFixture.ShouldNotExist<PersonalDetailsUpdatedEvent>(correlationId);

            var personalDetails = (await client.Profile.GetAsync(userId)).PersonalDetails;
            foreach (var property in patch.Properties)
            {
                var expectedValue = property.GetExpectedValue(userInfo, personalDetails);
                var actualValue = property.GetActualValue(personalDetails);
                actualValue.Should().BeEquivalentTo(expectedValue);
            }
        }

        /// <summary>
        /// Scenario: UserProfileUpdated event is raised
        /// Given user with personal details
        /// When UserProfileUpdated event is raised
        /// And it contains invalid residence address
        /// And all other fields is null
        /// Then address became unchanged
        /// And PersonalDetailsUpdated event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateResidenceAddress_WhenItInvalid(UserInfo userInfo, InvalidAddress invalidAddress)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var userProfileDto = new UserProfileDto
            {
                UserId = userId,
                ResidenceAddress = invalidAddress.SafeMap()
            };
            var userProfileUpdatedEvent = new UserProfileUpdatedEvent(new UserProfileEventArgs
            {
                UserProfile = userProfileDto,
                CorrelationId = correlationId
            }, correlationId);

            // Act
            await _eventsFixture.PublishAsync(userProfileUpdatedEvent);

            // Assert
            _eventsFixture.ShouldNotExist<PersonalDetailsUpdatedEvent>(correlationId);
            var profile = await client.Profile.GetAsync(userId);
            profile.PersonalDetails.ResidenceAddress.Should().BeEquivalentTo(userInfo.Address);
        }

        /// <summary>
        /// Scenario: UserProfileUpdated event is raised
        /// Given user with personal details
        /// When UserProfileUpdated event is raised
        /// And all it fields is null
        /// Then personal details became unchanged
        /// And PersonalDetailsUpdated event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdate_WhenAllFieldsIsNull(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var correlationId = Guid.NewGuid();
            var profileUpdatedEvent = new ProfileEventsBuilder(correlationId).BuildProfileUpdatedEvent();

            // Act
            await _eventsFixture.PublishAsync(profileUpdatedEvent);

            // Assert
            _eventsFixture.ShouldNotExist<PersonalDetailsUpdatedEvent>(correlationId);

            var personalDetails = (await client.Profile.GetAsync(userId)).PersonalDetails;
            using (new AssertionScope())
            {
                personalDetails.FirstName.Should().Be(userInfo.FirstName);
                personalDetails.LastName.Should().Be(userInfo.LastName);
                personalDetails.DateOfBirth.Should().Be(userInfo.DateOfBirth.Date);
                personalDetails.Email.Should().Be(userInfo.Email);
                personalDetails.Nationality.Should().Be(userInfo.Nationality);
                personalDetails.ResidenceAddress.Should().BeEquivalentTo(userInfo.Address);
            }
        }
    }
}
