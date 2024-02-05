using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.Risks.Events;
using WX.B2C.Risks.Events.Enums;
using WX.B2C.Risks.Events.EventArgs;
using WX.B2C.User.Verification.Api.Internal.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Events
{
    internal class RiskRatingChangedEventTests : BaseComponentTest
    {
        private ProfileFixture _profileFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private UserRiskLevelFixture _riskLevelFixture;
        private EventsFixture _eventsFixture;
        private IUserVerificationApiClientFactory _internalApiClientFactory;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _profileFixture = Resolve<ProfileFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _riskLevelFixture = Resolve<UserRiskLevelFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _internalApiClientFactory = Resolve<IUserVerificationApiClientFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<TwoDifferentArbitrary<RiskRating>>();
            Arb.Register<TaxResidenceArbitrary>();
        }

        /// <summary>
        /// Scenario: Set risk level
        /// Given user with verification details 
        /// And without risk level
        /// When RiskRatingChangedEvent is raised
        /// Then risk level is present 
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSetRiskLevel(UserInfo userInfo, TaxResidence taxResidence, RiskRating riskLevel)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(taxResidence));

            // Arrange
            var args = new RiskRatingChangedEventArgs
            {
                UserId = userId,
                RiskRating = riskLevel,
                PreviousRiskRating = null
            };
            var riskLevelEvent = new RiskRatingChangedEvent(userId.ToString(), args, Guid.NewGuid(), Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(riskLevelEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(riskLevelEvent.CorrelationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<RiskLevel?>(VerificationDetails.RiskLevel).Which;
            change.Should().Match(old => old.Should().BeNull(), @new => @new.Should().HaveSameNameAs(riskLevel));
        }

        /// <summary>
        /// Scenario: Update risk level
        /// Given user with verification details 
        /// And with risk level
        /// When RiskRatingChangedEvent is raised
        /// And risk level in event is different from current
        /// Then risk level is updated 
        /// And event is raised
        /// </summary>        
        [Theory]
        public async Task ShouldUpdateRiskLevel(UserInfo userInfo, TwoDifferent<RiskRating> two)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _riskLevelFixture.ChangeRiskLevelAsync(two.Item1, userId);

            // Arrange
            var args = new RiskRatingChangedEventArgs
            {
                UserId = userId,
                RiskRating = two.Item2,
                PreviousRiskRating = two.Item1
            };
            var riskLevelEvent = new RiskRatingChangedEvent(userId.ToString(), args, Guid.NewGuid(), Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(riskLevelEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(riskLevelEvent.CorrelationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<RiskLevel?>(VerificationDetails.RiskLevel).Which;
            change.Should().Match(old => old.Should().HaveSameNameAs(two.Item1), @new => @new.Should().HaveSameNameAs(two.Item2));
        }

        /// <summary>
        /// Scenario: Update risk level (same value)
        /// Given user with verification details 
        /// And with risk level
        /// When RiskRatingChangedEvent is raised
        /// And risk level in event is same as current
        /// Then risk level is not updated 
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotUpdateRiskLevel(UserInfo userInfo, RiskRating riskLevel)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _riskLevelFixture.ChangeRiskLevelAsync(riskLevel, userId);

            // Arrange
            var args = new RiskRatingChangedEventArgs
            {
                UserId = userId,
                RiskRating = riskLevel,
                PreviousRiskRating = riskLevel
            };
            var riskLevelEvent = new RiskRatingChangedEvent(userId.ToString(), args, Guid.NewGuid(), Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(riskLevelEvent);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(riskLevelEvent.CorrelationId);
        }

        /// <summary>
        /// Scenario: Create verification details
        /// Given user without verification details 
        /// When RiskRatingChangedEvent is raised
        /// Then verification details is created
        /// And risk level is present
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreateVerificationDetails(UserInfo userInfo, RiskRating riskLevel)
        {            
            // Given
            var userId = userInfo.UserId.Dump();
            var internalClient = _internalApiClientFactory.Create(Guid.NewGuid());
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var args = new RiskRatingChangedEventArgs
            {
                UserId = userId,
                RiskRating = riskLevel,
                PreviousRiskRating = null
            };
            var riskLevelEvent = new RiskRatingChangedEvent(userId.ToString(), args, Guid.NewGuid(), Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(riskLevelEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(riskLevelEvent.CorrelationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<RiskLevel?>(VerificationDetails.RiskLevel).Which;
            change.Should().Match(old => old.Should().BeNull(), @new => @new.Should().HaveSameNameAs(riskLevel));

            var profile = await internalClient.Profile.GetAsync(userId);
            var actualRiskLevel = profile?.VerificationDetails?.RiskLevel;
            actualRiskLevel.Should().HaveSameNameAs(riskLevel);
        }
    }
}
