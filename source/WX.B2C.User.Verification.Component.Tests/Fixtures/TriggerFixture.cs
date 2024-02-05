using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class TriggerFixture
    {
        private readonly ApplicationFixture _applicationFixture;
        private readonly ITriggerProvider _triggerProvider;
        private readonly EventsFixture _eventsFixture;

        public TriggerFixture(ApplicationFixture applicationFixture, EventsFixture eventsFixture, ITriggerProvider triggerProvider)
        {
            _applicationFixture = applicationFixture ?? throw new ArgumentNullException(nameof(applicationFixture));
            _triggerProvider = triggerProvider ?? throw new ArgumentNullException(nameof(triggerProvider));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public void ScheduleOnboardingTriggers(Guid applicationId, VerificationPolicy policy, Seed seed)
        {
            var monitoringTriggers = _triggerProvider.GetOnbording(policy);
            monitoringTriggers.Foreach(IsScheculed);

            void IsScheculed(Guid variantId) =>
                _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(
                    e => e.EventArgs.VariantId == variantId && e.EventArgs.ApplicationId == applicationId);
        }

        public async Task ScheduleMonitoringTriggersAsync(Guid userId, Guid applicationId, VerificationPolicy policy, Seed seed)
        {
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            var monitoringTriggers = _triggerProvider.GetMonitoring(policy);
            monitoringTriggers.Foreach(IsScheculed);

            void IsScheculed(Guid variantId) =>
                _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(
                    e => e.EventArgs.VariantId == variantId && e.EventArgs.ApplicationId == applicationId);
        }
    }
}
