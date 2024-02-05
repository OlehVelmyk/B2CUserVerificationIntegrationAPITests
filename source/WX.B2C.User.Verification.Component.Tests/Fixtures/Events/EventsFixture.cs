using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Polly;
using Polly.Retry;
using WX.Domain.Definitions;
using WX.Messaging.Core;
using WX.Messaging.Publisher.Services;
using WX.Messaging.Stub;
using WX.Messaging.Stub.Publisher;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures.Events
{
    internal class EventsFixture
    {
        private const int OperationTimeout = 100;
        private static int MaxRetryCount = Debugger.IsAttached ? 50 : 30;
        private static readonly TimeSpan DefaultDelayToCheckAbsence = TimeSpan.FromSeconds(2);

        private readonly StubPublisher _publisherStub;
        private readonly EventStorage _eventStorage = EventStorage.Instance;

        public EventsFixture()
        {
            var storage = new SharedInMemoryEventStorage();
            _publisherStub = new StubPublisher(storage, new PublisherChannelNameResolver());
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            await _publisherStub.PublishAsync(@event);
        }

        public TEvent ShouldExist<TEvent>(Func<TEvent, bool> predicate) where TEvent : Event
        {
            return WaitUntil(() =>
            {
                var conditionalValue = TryGetEvent(predicate);

                if (!conditionalValue.HasValue)
                    throw new EventNotFoundException<TEvent>();
                
                return conditionalValue.Value;
            });
        }

        public TEvent ShouldExistSingle<TEvent>(Func<TEvent, bool> predicate) where TEvent : Event
        {
            return WaitUntil(() =>
            {
                TEvent result = null;
                foreach (var @event in GetAllEvents(predicate))
                {
                    if (result != null)
                        throw new ManyEventsFoundException<TEvent>();

                    result = @event;
                }

                if (result == null)
                    throw new EventNotFoundException<TEvent>();

                return result;
            });
        }
        
        // TODO: Consider if all events should have unique correlationId
        public TEvent[] ShouldExistExact<TEvent>(int amount, Func<TEvent, bool> predicate) where TEvent : Event
        {
            return WaitUntil(() =>
            {
                var events = GetAllEvents(predicate).ToArray();
                var unique = events.Select(e => e.CorrelationId).Distinct().ToArray();
                var (actual, expected) = (unique.Length, amount);
                if (actual != expected)
                    throw new EventNotFoundException<TEvent>(actual, expected);

                return unique
                .Select(correlationId => events.First(e => e.CorrelationId == correlationId))
                .OrderBy(e => e.CreatedOn)
                .ToArray();
            });
        }

        public void ShouldNotExist<TEvent>(Func<TEvent, bool> predicate) where TEvent : Event
        {
            WaitAndCheck(() =>
            {
                var conditionalValue = TryGetEvent(predicate);

                if (conditionalValue.HasValue)
                    throw new UnexpectedEventPublished<TEvent>();
                conditionalValue.HasValue.Should().BeFalse();
            });
        }

        public void ShouldNotExistExact<TEvent>(int amount, Func<TEvent, bool> predicate) where TEvent : Event
        {
            WaitAndCheck(() =>
            {
                var events = GetAllEvents(predicate).ToArray();
                var unique = events.Select(e => e.CorrelationId).Distinct().ToArray();
                var (actual, expected) = (unique.Length, amount - 1);
                if (actual != expected)
                    throw new UnexpectedEventPublished<TEvent>(actual, expected);
            });
        }

        public IEnumerable<TEvent> GetAllEvents<TEvent>(Func<TEvent, bool> predicate) where TEvent : Event
        {
            foreach (var @event in _eventStorage.GetAllEvents<TEvent>())
            {
                if (predicate(@event))
                    yield return @event;
            }
        }

        private ConditionalValue<TEvent> TryGetEvent<TEvent>(Func<TEvent, bool> predicate) where TEvent : Event
        {
            foreach (var @event in GetAllEvents(predicate))
            {
                return ConditionalValue<TEvent>.WithValue(@event);
            }

            return ConditionalValue<TEvent>.WithNoValue();
        }

        /// <summary>
        /// Due to verification service is reactive service and we don't know when operation will be finished,
        /// we should use retry policy to understand that operation is finished.
        /// </summary>
        /// <param name="arrange">Put some arrange criteria here which indicates that operation is finished,
        /// otherwise must throw <see cref="EventNotFoundException"/>
        /// </param>
        private TEvent WaitUntil<TEvent>(Func<TEvent> arrange) where TEvent : Event
        {
            var policy = CreatePolicy();
            return policy.Execute(arrange);
        }

        private TEvent[] WaitUntil<TEvent>(Func<TEvent[]> arrange) where TEvent : Event
        {
            var policy = CreatePolicy();
            return policy.Execute(arrange);
        }

        private void WaitAndCheck(Action arrange)
        {
            Thread.Sleep(DefaultDelayToCheckAbsence);
            arrange();
        }

        private RetryPolicy CreatePolicy()
        {
            return Policy
                   .Handle<EventNotFoundException>()
                   .WaitAndRetry(MaxRetryCount,
                                 i =>
                                 {
                                     Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.ffff}:{i} iteration failed");
                                     return TimeSpan.FromMilliseconds(OperationTimeout * i);
                                 });
        }
    }

    internal class EventNotFoundException : Exception
    {
        public EventNotFoundException(string message)
            : base(message) { }
    }

    internal class EventNotFoundException<T> : EventNotFoundException
    {
        public EventNotFoundException()
            : base($"Event {typeof(T).Name} is not found") { }

        public EventNotFoundException(int actual, int expected)
            : base($"{expected} events {typeof(T).Name} are not found, only {actual}.") { }
    }

    internal class UnexpectedEventPublished<T> : EventNotFoundException
    {
        public UnexpectedEventPublished()
            : base($"Event {typeof(T).Name} was published") { }

        public UnexpectedEventPublished(int actual, int expected)
            : base($"Event {typeof(T).Name} was published {actual} times when expected {expected}.") { }
    }

    internal class ManyEventsFoundException<T> : Exception
    { 
        public override string Message => $"Found more than one event {typeof(T).Name} when expected single";
    }
}
