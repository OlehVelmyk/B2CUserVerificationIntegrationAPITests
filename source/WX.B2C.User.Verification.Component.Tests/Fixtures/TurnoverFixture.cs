using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.Risk.Assessment.Transaction.Dto;
using WX.Risk.Assessment.Transaction.Events.EventArgs;
using WX.Risk.Assessment.Transaction.Events.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class TurnoverFixture
    {
        private readonly EventsFixture _eventsFixture;

        public TurnoverFixture(EventsFixture eventsFixture)
        {
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public async Task UpdateAsync(Guid userId, decimal turnover)
        {
            var args = new StandaloneRiskFactorUpdatedEventArgs
            {
                OwnerId = userId,
                RiskFactor = new RiskFactorDto
                {
                    Name = RiskFactors.AllTheTimeIncomeTurnoverRiskFactor,
                    Weight = turnover
                }
            };
            var turnoverEvent = new StandaloneRiskFactorUpdatedEvent(args, Guid.NewGuid());
            await _eventsFixture.PublishAsync(turnoverEvent);

            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(isTurnoverUpdated);

            bool isTurnoverUpdated(VerificationDetailsUpdatedEvent e) =>
                e.CorrelationId == turnoverEvent.CorrelationId && 
                e.EventArgs.Changes.Find<decimal?>(VerificationDetails.Turnover) is not null;
        }
    }
}
