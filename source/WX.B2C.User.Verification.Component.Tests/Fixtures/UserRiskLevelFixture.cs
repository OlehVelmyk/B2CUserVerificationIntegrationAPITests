using System;
using System.Threading.Tasks;
using WX.B2C.Risks.Events;
using WX.B2C.Risks.Events.Enums;
using WX.B2C.Risks.Events.EventArgs;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class UserRiskLevelFixture
    {
        private readonly EventsFixture _eventsFixture = new();
        
        /// <remarks>
        /// If new risk level is same as current then exception will be thrown
        /// </remarks>
        public async Task ChangeRiskLevelAsync(RiskRating riskLevel, Guid userId, Guid? correlationId = null)
        {
            correlationId ??= Guid.NewGuid();
            var riskRatingChangedEventArgs = new RiskRatingChangedEventArgs
            {
                UserId = userId,
                RiskRating = riskLevel
            };
            var ratingChangedEvent = new RiskRatingChangedEvent(userId.ToString(),
                                                                riskRatingChangedEventArgs,
                                                                Guid.NewGuid(),
                                                                correlationId);
            
            await _eventsFixture.PublishAsync(ratingChangedEvent);
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(isRiskLevelUpdated);

            bool isRiskLevelUpdated(VerificationDetailsUpdatedEvent e) =>
                e.CorrelationId == correlationId &&
                e.EventArgs.Changes.Find<RiskLevel?>(VerificationDetails.RiskLevel) is not null;
        }
    }
}