using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;
using ApplicationRegisteredEvent = WX.B2C.User.Verification.Events.Events.ApplicationRegisteredEvent;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class AuditEventHandler : BaseEventHandler,
                                       IEventHandler<ApplicationRegisteredEvent>,
                                       IEventHandler<ApplicationStateChangedEvent>,
                                       IEventHandler<ApplicationRequiredTaskAddedEvent>,
                                       IEventHandler<CheckCreatedEvent>,
                                       IEventHandler<CheckCompletedEvent>,
                                       IEventHandler<CollectionStepRequestedEvent>,
                                       IEventHandler<CollectionStepCompletedEvent>,
                                       IEventHandler<PersonalDetailsUpdatedEvent>,
                                       IEventHandler<VerificationDetailsUpdatedEvent>,
                                       IEventHandler<TaskCreatedEvent>,
                                       IEventHandler<TaskCompletedEvent>,
                                       IEventHandler<TaskIncompleteEvent>,
                                       IEventHandler<TaskCollectionStepAddedEvent>,
                                       IEventHandler<DocumentSubmittedEvent>
    {
        private readonly IAuditService _auditService;
        private readonly IAuditModelsMapper _auditModelsMapper;

        public AuditEventHandler(
            IAuditService auditService,
            IAuditModelsMapper auditModelsMapper,
            EventHandlingContext context) : base(context)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _auditModelsMapper = auditModelsMapper ?? throw new ArgumentNullException(nameof(auditModelsMapper));
        }

        public Task HandleAsync(ApplicationRegisteredEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(ApplicationRequiredTaskAddedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(CheckCreatedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(CheckCompletedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(CollectionStepCompletedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(TaskCreatedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(TaskCompletedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(TaskIncompleteEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(TaskCollectionStepAddedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });

        public Task HandleAsync(DocumentSubmittedEvent @event) =>
            Handle(@event, args =>
            {
                var dto = _auditModelsMapper.Map(args, @event.CreatedOn);
                return _auditService.SaveAsync(dto);
            });
    }
}
