using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    public interface IEventMapperFactory
    {
        Func<DomainEvent, Event[]> GetMapper(Type eventType);
    }

    internal class EventMapperFactory : IEventMapperFactory
    {
        private readonly ConcurrentDictionary<string, ImmutableArray<Func<DomainEvent, Event>>> _mappers = new();

        public EventMapperFactory(IInnerEventMapper innerEventMapper, IIntegrationEventMapper integrationEventMapper)
        {
            if (innerEventMapper == null)
                throw new ArgumentNullException(nameof(innerEventMapper));
            if (integrationEventMapper == null)
                throw new ArgumentNullException(nameof(integrationEventMapper));

            // Internal events
            Register<ApplicationRequiredTaskAdded>(innerEventMapper.Map);
            Register<ApplicationStateChanged>(innerEventMapper.Map);
			Register<ApplicationAutomated>(innerEventMapper.Map);
            Register<CheckCreated>(innerEventMapper.Map);
            Register<CheckStarted>(innerEventMapper.Map);
            Register<CheckPerformed>(innerEventMapper.Map);
            Register<CheckCompleted>(innerEventMapper.Map);
            Register<CheckErrorOccurred>(innerEventMapper.Map);
            Register<CollectionStepCompleted>(innerEventMapper.Map);
            Register<CollectionStepRequested>(innerEventMapper.Map);
            Register<CollectionStepRequired>(innerEventMapper.Map);
            Register<CollectionStepReadyForReview>(innerEventMapper.Map);
            Register<DocumentSubmitted>(innerEventMapper.Map);
            Register<PersonalDetailsUpdated>(innerEventMapper.Map);
            Register<TaskCollectionStepAdded>(innerEventMapper.Map);
            Register<TaskCollectionStepRemoved>(innerEventMapper.Map);
            Register<TaskCompleted>(innerEventMapper.Map);
            Register<TaskCreated>(innerEventMapper.Map);
            Register<TaskIncomplete>(innerEventMapper.Map);
            Register<VerificationDetailsUpdated>(innerEventMapper.Map);
            Register<TriggerFired>(innerEventMapper.Map);
            Register<TriggerScheduled>(innerEventMapper.Map);
            Register<TriggerUnscheduled>(innerEventMapper.Map);
            Register<TriggerCompleted>(innerEventMapper.Map);
            Register<ScheduledTriggerJobFinished>(innerEventMapper.Map);
            Register<UserTriggersActionRequired>(innerEventMapper.Map);
            Register<ExternalProfileCreated>(innerEventMapper.Map);
            Register<CollectionStepUpdated>(innerEventMapper.Map);
            Register<UserReminderJobFinished>(innerEventMapper.Map);

            // Public events
            Register<ApplicationRegistered>(integrationEventMapper.Map);
            Register<ApplicationStateChanged>(integrationEventMapper.Map);
            Register<CheckCompleted>(integrationEventMapper.Map);
            Register<CollectionStepCompleted>(integrationEventMapper.Map);
            Register<CollectionStepRequested>(integrationEventMapper.Map);
            Register<CollectionStepReadyForReview>(integrationEventMapper.Map);
            Register<VerificationDetailsUpdated>(integrationEventMapper.Map);

            Skip<DocumentArchived>();
            Skip<CollectionStepReviewRequested>();
            Skip<CollectionStepCancelled>();
            Skip<CheckCancelled>();
        }

        public Func<DomainEvent, Event[]> GetMapper(Type eventType)
        {
            var fullEventName = eventType.FullName ?? throw new InvalidOperationException();

            if (!_mappers.TryGetValue(fullEventName, out var mappers))
                throw new InvalidCastException(eventType.FullName);

            return domainEvent => mappers
                                  .Select(mapper => mapper(domainEvent))
                                  .Where(@event => @event != null)
                                  .ToArray();
        }

        private void Skip<TEvent>() where TEvent : DomainEvent => Register<TEvent>(_ => null);

        private void Register<TEvent>(Func<TEvent, Event> mapper) where TEvent : DomainEvent
        {
            var fullEventName = typeof(TEvent).FullName ?? throw new InvalidOperationException();

            Event InnerMapper(DomainEvent eventArgs)
            {
                if (!(eventArgs is TEvent castedEventArgs))
                    throw new InvalidCastException(typeof(TEvent).FullName);

                return mapper(castedEventArgs);
            }

            _mappers.AddOrUpdate(fullEventName,
                ImmutableArray.Create((Func<DomainEvent, Event>)InnerMapper),
                (_, mappers) => mappers.Add(InnerMapper));
        }
    }
}
