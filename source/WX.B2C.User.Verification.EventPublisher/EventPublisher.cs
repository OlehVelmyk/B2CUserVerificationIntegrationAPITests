using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.EventPublisher.Mappers;
using WX.B2C.User.Verification.Extensions;
using IEventHubPublisher = WX.Messaging.Publisher.IEventPublisher;

namespace WX.B2C.User.Verification.EventPublisher
{
    internal class EventPublisher:IEventPublisher
    {
        private readonly IEventHubPublisher _publisher;
        private readonly IEventMapperFactory _eventMapperFactory;
        private readonly IEventDataFiller _eventDataFiller;

        public EventPublisher(IEventHubPublisher eventPublisher, IEventMapperFactory eventMapperFactory, IEventDataFiller eventDataFiller)
        {
            _publisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _eventMapperFactory = eventMapperFactory ?? throw new ArgumentNullException(nameof(eventMapperFactory));
            _eventDataFiller = eventDataFiller ?? throw new ArgumentNullException(nameof(eventDataFiller));
        }

        public async Task PublishAsync(params DomainEvent[] events)
        {
            foreach (var @event in events)
            {
                await PublishAsync(@event);
            }
        }

        private async Task PublishAsync(DomainEvent @event)
        {
            var eventType = @event.GetType();

            var eventMapper = _eventMapperFactory.GetMapper(eventType);

            var events = eventMapper.Invoke(@event);

            await events.Foreach(e => _eventDataFiller.FillAsync(e));
            await events.Foreach(e => _publisher.PublishAsync(e));
        }
    }
}