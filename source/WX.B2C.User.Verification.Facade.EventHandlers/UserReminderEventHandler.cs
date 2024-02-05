using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class UserReminderEventHandler : BaseEventHandler,
                                              IEventHandler<CollectionStepRequestedEvent>,
                                              IEventHandler<CollectionStepCompletedEvent>,
                                              IEventHandler<CollectionStepReadyForReviewEvent>,
                                              IEventHandler<CollectionStepRequiredEvent>,
                                              IEventHandler<CollectionStepUpdatedEvent>,
                                              IEventHandler<ApplicationAutomatedEvent>,
                                              IEventHandler<UserReminderJobFinishedEvent>,
                                              IEventHandler<ApplicationStateChangedEvent>
    {
        private readonly IReminderEventObserver _eventObserver;

        public UserReminderEventHandler(IReminderEventObserver eventObserver,
                                        EventHandlingContext context) : base(context)
        {
            _eventObserver = eventObserver ?? throw new ArgumentNullException(nameof(eventObserver));
        }

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnCollectionStepStateChanged(args.UserId, args.CollectionStepId));

        public Task HandleAsync(CollectionStepRequiredEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnCollectionStepStateChanged(args.UserId, args.CollectionStepId));

        public Task HandleAsync(CollectionStepUpdatedEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnUserActionsChanged(args.UserId));

        public Task HandleAsync(CollectionStepCompletedEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnCollectionStepSubmitted(args.UserId, args.CollectionStepId));

        public Task HandleAsync(CollectionStepReadyForReviewEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnCollectionStepSubmitted(args.UserId, args.CollectionStepId));

        public Task HandleAsync(ApplicationAutomatedEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnUserActionsChanged(args.UserId));

        public Task HandleAsync(UserReminderJobFinishedEvent @event) =>
            Handle(@event,
                   args => _eventObserver.OnJobFinishedAsync(args.UserId, args.Id));

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event,
                args => args.NewState switch
                {
                    ApplicationState.Rejected or ApplicationState.Cancelled => _eventObserver.OnApplicationRejected(args.UserId),
                    _ when args.PreviousState is ApplicationState.Rejected or ApplicationState.Cancelled => _eventObserver.OnApplicationReverted(args.UserId),
                    _ => Task.CompletedTask
                });
    }
}