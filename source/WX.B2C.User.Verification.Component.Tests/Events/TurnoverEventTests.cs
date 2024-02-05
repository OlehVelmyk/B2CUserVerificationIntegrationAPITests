using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Risk.Assessment.Transaction.Dto;
using WX.Risk.Assessment.Transaction.Events.EventArgs;
using WX.Risk.Assessment.Transaction.Events.Events;

namespace WX.B2C.User.Verification.Component.Tests.Events
{
    internal class TurnoverEventTests : BaseComponentTest
    {
        private ProfileFixture _profileFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private TurnoverFixture _turnoverFixture;
        private EventsFixture _eventsFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _profileFixture = Resolve<ProfileFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _turnoverFixture = Resolve<TurnoverFixture>();
            _eventsFixture = Resolve<EventsFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<TaxResidenceArbitrary>();
            Arb.Register<PositiveDecimalArbitrary>();
            Arb.Register<TwoDifferentArbitrary<PositiveDecimal>>();
        }

        /// <summary>
        /// Scenario: StandaloneRiskFactorUpdated event is raised
        /// Given user with undefined verification details
        /// When StandaloneRiskFactorUpdated event is raised
        /// And event risk factor is "AllTheTimeIncomeTurnoverRiskFactor"
        /// Then verification details is created
        /// And event VerificationDetailsUpdated event is raised
        /// And it contains turnover update
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTurnover_WhenVerificationDetailsIsUndefined(UserInfo userInfo, PositiveDecimal weight)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var args = new StandaloneRiskFactorUpdatedEventArgs
            {
                OwnerId = userId,
                RiskFactor = new RiskFactorDto
                {
                    Name = RiskFactors.AllTheTimeIncomeTurnoverRiskFactor,
                    Weight = weight
                }
            };
            var turnoverEvent = new StandaloneRiskFactorUpdatedEvent(args, Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(turnoverEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(turnoverEvent.CorrelationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<decimal?>(VerificationDetails.Turnover).Which;
            change.Should().Match(old => old.Should().BeNull(), @new => @new.Should().BeApproximately(weight, 0.01m));
        }

        /// <summary>
        /// Scenario: StandaloneRiskFactorUpdated event is raised
        /// Given user with defined verification details
        /// And turnover property is set
        /// When StandaloneRiskFactorUpdated event is raised
        /// And event risk factor is "AllTheTimeIncomeTurnoverRiskFactor"
        /// And event turnover value differ given
        /// Then verification details is updated
        /// And event VerificationDetailsUpdated is raised
        /// And it contains turnover update
        /// </summary>
        [Theory]
        public async Task ShouldUpdateTurnover_WhenVerificationDetailsIsDefined(UserInfo userInfo, TwoDifferent<PositiveDecimal> weights)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _turnoverFixture.UpdateAsync(userId, weights.Item1);
            
            // Arrange
            var args = new StandaloneRiskFactorUpdatedEventArgs
            {
                OwnerId = userId,
                RiskFactor = new RiskFactorDto
                {
                    Name = RiskFactors.AllTheTimeIncomeTurnoverRiskFactor,
                    Weight = weights.Item2
                }
            };
            var turnoverEvent = new StandaloneRiskFactorUpdatedEvent(args, Guid.NewGuid());
            
            // Act
            await _eventsFixture.PublishAsync(turnoverEvent);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(turnoverEvent.CorrelationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<decimal?>(VerificationDetails.Turnover).Which;
            change.Should().Match(old => old.Should().BeApproximately(weights.Item1, 0.01m), 
                                  @new => @new.Should().BeApproximately(weights.Item2, 0.01m));    
        }

        /// <summary>
        /// Scenario: StandaloneRiskFactorUpdated event is raised
        /// Given user with defined verification details
        /// And turnover property is not set
        /// When StandaloneRiskFactorUpdated event is raised
        /// And event risk factor is not "AllTheTimeIncomeTurnoverRiskFactor"
        /// Then verification details should not be changed
        /// And event VerificationDetailsUpdated is not raised
        /// </summary>
        [Theory]
        public async Task ShouldDoNothing_WhenEventContainsNotTurnoverRiskFactor(UserInfo userInfo, TaxResidence taxResidence, PositiveDecimal weight)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(taxResidence));

            // Arrange
            var args = new StandaloneRiskFactorUpdatedEventArgs
            {
                OwnerId = userId,
                RiskFactor = new RiskFactorDto
                {
                    Name = nameof(ShouldDoNothing_WhenEventContainsNotTurnoverRiskFactor),
                    Weight = weight
                }
            };
            var turnoverEvent = new StandaloneRiskFactorUpdatedEvent(args, Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(turnoverEvent);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(turnoverEvent.CorrelationId);
        }

        /// <summary>
        /// Scenario: StandaloneRiskFactorUpdated event is raised
        /// Given user with defined verification details
        /// And turnover property is set to given value
        /// When StandaloneRiskFactorUpdated event is raised
        /// And event risk factor is "AllTheTimeIncomeTurnoverRiskFactor"
        /// And event turnover value is equal to given
        /// Then verification details should not be changed
        /// And event VerificationDetailsUpdated is not raised
        /// </summary>
        [Theory]
        public async Task ShouldDoNothing_WhenEventContainsTurnoverEqualToCurrent(UserInfo userInfo, PositiveDecimal weight)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);
            await _turnoverFixture.UpdateAsync(userId, weight);

            // Arrange
            var args = new StandaloneRiskFactorUpdatedEventArgs
            {
                OwnerId = userId,
                RiskFactor = new RiskFactorDto
                {
                    Name = RiskFactors.AllTheTimeIncomeTurnoverRiskFactor,
                    Weight = weight
                }
            };
            var turnoverEvent = new StandaloneRiskFactorUpdatedEvent(args, Guid.NewGuid());

            // Act
            await _eventsFixture.PublishAsync(turnoverEvent);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(turnoverEvent.CorrelationId);
        }
    }
}