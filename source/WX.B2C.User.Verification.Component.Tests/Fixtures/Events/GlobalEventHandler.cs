using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Events.Events;
using WX.Messaging.Core;
using WX.Messaging.Subscriber.HandlerResolving;
using InternalEvents = WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures.Events
{
    internal class GlobalEventHandler
        : IEventHandler<ApplicationRegisteredEvent>,
          IEventHandler<ApplicationStateChangedEvent>,
          IEventHandler<InternalEvents.ApplicationStateChangedEvent>,
          IEventHandler<InternalEvents.ApplicationAutomatedEvent>,
          IEventHandler<InternalEvents.CheckCompletedEvent>,
          IEventHandler<InternalEvents.CheckCreatedEvent>,
          IEventHandler<InternalEvents.CheckErrorOccuredEvent>,
          IEventHandler<InternalEvents.CollectionStepCompletedEvent>,
          IEventHandler<InternalEvents.CollectionStepRequestedEvent>,
          IEventHandler<InternalEvents.CollectionStepRequiredEvent>,
          IEventHandler<InternalEvents.CollectionStepReadyForReviewEvent>,
          IEventHandler<InternalEvents.CollectionStepUpdatedEvent>,
          IEventHandler<InternalEvents.PersonalDetailsUpdatedEvent>,
          IEventHandler<InternalEvents.VerificationDetailsUpdatedEvent>,
          IEventHandler<InternalEvents.TaskCompletedEvent>,
          IEventHandler<InternalEvents.TaskIncompleteEvent>,
          IEventHandler<InternalEvents.TaskCreatedEvent>,
          IEventHandler<InternalEvents.DocumentSubmittedEvent>,
          IEventHandler<InternalEvents.ApplicationRequiredTaskAddedEvent>,
          IEventHandler<InternalEvents.TriggerFiredEvent>,
          IEventHandler<InternalEvents.TriggerScheduledEvent>,
          IEventHandler<InternalEvents.TriggerUnscheduledEvent>,
          IEventHandler<InternalEvents.TriggerCompletedEvent>,
          IEventHandler<InternalEvents.TaskCollectionStepAddedEvent>,
          IEventHandler<InternalEvents.CheckStartedEvent>,
          IEventHandler<InternalEvents.CheckPerformedEvent>,
          IEventHandler<InternalEvents.UserReminderJobFinishedEvent>
    {
        private static readonly Dictionary<string, IProducerConsumerCollection<Event>> Events =
            new()
            {
                { GetKey<ApplicationRegisteredEvent>(), new ConcurrentStack<Event>() },
                { GetKey<ApplicationStateChangedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.ApplicationStateChangedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.ApplicationAutomatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CheckCreatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CheckCompletedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CheckErrorOccuredEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CollectionStepCompletedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CollectionStepRequestedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CollectionStepRequiredEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CollectionStepReadyForReviewEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CollectionStepUpdatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.PersonalDetailsUpdatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.VerificationDetailsUpdatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TaskCompletedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TaskIncompleteEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TaskCreatedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.DocumentSubmittedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.ApplicationRequiredTaskAddedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TriggerFiredEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TriggerScheduledEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TriggerUnscheduledEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TriggerCompletedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.TaskCollectionStepAddedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CheckStartedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.CheckPerformedEvent>(), new ConcurrentStack<Event>() },
                { GetKey<InternalEvents.UserReminderJobFinishedEvent>(), new ConcurrentStack<Event>() }
            };

        private static Task StoreEvent(Event @event)
        {
            var events = Events[GetKey(@event)];
            events.TryAdd(@event);
            return Task.CompletedTask;
        }

        public IEnumerable<TEvent> GetAllEvents<TEvent>() where TEvent : Event
        {
            return Events[GetKey<TEvent>()].ToArray().Cast<TEvent>();
        }

        public Task HandleAsync(ApplicationRegisteredEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(ApplicationStateChangedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.ApplicationStateChangedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.ApplicationAutomatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CheckCompletedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CheckCreatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CheckErrorOccuredEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CollectionStepCompletedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CollectionStepRequestedEvent message) =>
            StoreEvent(message);
        
        public Task HandleAsync(InternalEvents.CollectionStepRequiredEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CollectionStepReadyForReviewEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CollectionStepUpdatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.PersonalDetailsUpdatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.VerificationDetailsUpdatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TaskCompletedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TaskIncompleteEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TaskCreatedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.DocumentSubmittedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.ApplicationRequiredTaskAddedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TriggerFiredEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TriggerScheduledEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TriggerUnscheduledEvent message) =>
            StoreEvent(message);
			
        public Task HandleAsync(InternalEvents.TriggerCompletedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.TaskCollectionStepAddedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CheckStartedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.CheckPerformedEvent message) =>
            StoreEvent(message);

        public Task HandleAsync(InternalEvents.UserReminderJobFinishedEvent message) =>
            StoreEvent(message);

        private static string GetKey<TEvent>() where TEvent : Event =>
            typeof(TEvent).FullName;

        private static string GetKey(Event message) =>
            message.GetType().FullName;
    }
}
