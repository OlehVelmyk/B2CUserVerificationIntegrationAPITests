using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class TriggersEventHandler : BaseEventHandler,
                                          IEventHandler<ScheduledTriggerJobFinishedEvent>,
                                          IEventHandler<ApplicationStateChangedEvent>,
                                          IEventHandler<TriggerScheduledEvent>,
                                          IEventHandler<UserTriggersActionRequiredEvent>,
                                          IEventHandler<ApplicationAutomatedEvent>,
                                          IEventHandler<TriggerCompletedEvent>
    {
        private readonly ITriggerEventObserver _triggerEventObserver;

        public TriggersEventHandler(
            ITriggerEventObserver triggerEventObserver,
            EventHandlingContext context) : base(context)
        {
            _triggerEventObserver = triggerEventObserver ?? throw new ArgumentNullException(nameof(triggerEventObserver));
        }

        public Task HandleAsync(ScheduledTriggerJobFinishedEvent @event) =>
            Handle(@event, args => _triggerEventObserver.OnTriggerReadyToFire(args.TriggerId));

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args => _triggerEventObserver.OnApplicationStateChanged(
                args.UserId,
                args.ApplicationId,
                args.PreviousState.To<ApplicationState>(),
                args.NewState.To<ApplicationState>()));

        public Task HandleAsync(TriggerScheduledEvent @event) =>
            Handle(@event, args => _triggerEventObserver.OnTriggerScheduled(args.VariantId, args.TriggerId));

        public Task HandleAsync(UserTriggersActionRequiredEvent @event) =>
            Handle(@event, args => args.Actions.IsNullOrEmpty()
                ? throw new ArgumentNullException(nameof(args.Actions))
                : _triggerEventObserver.OnActionsRequested(args.UserId, args.Actions, args.TriggerPolicyId));

        /// <summary>
        /// TODO WRXB-10546 Remove in phase 2 when all users will be migrated
        /// </summary>
        public Task HandleAsync(ApplicationAutomatedEvent @event) =>
            Handle(@event, args => _triggerEventObserver.OnApplicationAutomated(args.UserId, args.ApplicationId));

        public Task HandleAsync(TriggerCompletedEvent @event) =>
            Handle(@event, args => _triggerEventObserver.OnTriggerCompleted(args.UserId, args.ApplicationId, args.VariantId));
    }
}