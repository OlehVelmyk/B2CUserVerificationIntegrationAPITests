using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Events.EventArgs;
using WX.B2C.User.Verification.Events.Events;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.B2C.User.Verification.Events.Internal.Events;
using ApplicationStateChangedEvent = WX.B2C.User.Verification.Events.Internal.Events.ApplicationStateChangedEvent;
using ApplicationStateChangedEventArgs = WX.B2C.User.Verification.Events.Internal.EventArgs.ApplicationStateChangedEventArgs;
using CheckCompletedEvent = WX.B2C.User.Verification.Events.Internal.Events.CheckCompletedEvent;
using CheckCompletedEventArgs = WX.B2C.User.Verification.Events.Internal.EventArgs.CheckCompletedEventArgs;
using CollectionStepCompletedEvent = WX.B2C.User.Verification.Events.Internal.Events.CollectionStepCompletedEvent;
using CollectionStepCompletedEventArgs = WX.B2C.User.Verification.Events.Internal.EventArgs.CollectionStepCompletedEventArgs;
using CollectionStepRequestedEvent = WX.B2C.User.Verification.Events.Internal.Events.CollectionStepRequestedEvent;
using CollectionStepRequestedEventArgs = WX.B2C.User.Verification.Events.Internal.EventArgs.CollectionStepRequestedEventArgs;
using VerificationDetailsUpdatedEvent = WX.B2C.User.Verification.Events.Internal.Events.VerificationDetailsUpdatedEvent;
using VerificationDetailsUpdatedEventArgs = WX.B2C.User.Verification.Events.Internal.EventArgs.VerificationDetailsUpdatedEventArgs;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface IAuditModelsMapper
    {
        AuditEntryDto Map(VerificationApplicationRegisteredEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(ApplicationStateChangedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(ApplicationRequiredTaskAddedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(CheckCreatedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(CheckCompletedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(CollectionStepRequestedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(CollectionStepCompletedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(PersonalDetailsUpdatedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(VerificationDetailsUpdatedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(TaskCreatedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(TaskCompletedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(TaskIncompleteEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(TaskCollectionStepAddedEventArgs eventArgs, DateTime createdAt);

        AuditEntryDto Map(DocumentSubmittedEventArgs eventArgs, DateTime createdAt);
    }

    internal class AuditModelsMapper : IAuditModelsMapper
    {
        private readonly IInitiationMapper _initiationMapper;
        private readonly IAuditDataSerializer _dataSerializer;

        public AuditModelsMapper(IInitiationMapper initiationMapper,
                                 IAuditDataSerializer dataSerializer)
        {
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
        }

        public AuditEntryDto Map(VerificationApplicationRegisteredEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            // TODO: Remove after release 'Country change flow'
            var initiationDto = eventArgs.Initiation != null
                ? _initiationMapper.Map(eventArgs.Initiation)
                : InitiationDto.CreateSystem("Application created when verification");

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.ApplicationId,
                EntryType.Application,
                nameof(ApplicationRegisteredEvent), createdAt,
                ToJson(new { eventArgs.PolicyId, eventArgs.ProductType }),
                initiationDto);
        }

        public AuditEntryDto Map(ApplicationStateChangedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.ApplicationId,
                EntryType.Application,
                nameof(ApplicationStateChangedEvent),
                createdAt,
                ToJson(new { eventArgs.PreviousState, eventArgs.NewState }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(ApplicationRequiredTaskAddedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.ApplicationId,
                EntryType.Application,
                nameof(ApplicationRequiredTaskAddedEvent),
                createdAt,
                ToJson(new { eventArgs.TaskId }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(CheckCreatedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));


            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.CheckId,
                EntryType.Check,
                nameof(CheckCreatedEvent),
                createdAt,
                ToJson(new { eventArgs.VariantId, eventArgs.RelatedTasksId }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(CheckCompletedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var initiationDto = InitiationDto.CreateSystem("Check completed");

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.CheckId,
                EntryType.Check,
                nameof(CheckCompletedEvent),
                createdAt,
                ToJson(new { eventArgs.VariantId, eventArgs.Type, eventArgs.Result }),
                initiationDto);
        }

        public AuditEntryDto Map(CollectionStepRequestedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.CollectionStepId,
                EntryType.CollectionStep,
                nameof(CollectionStepRequestedEvent),
                createdAt,
                ToJson(new { eventArgs.XPath }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(CollectionStepCompletedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.CollectionStepId,
                EntryType.CollectionStep,
                nameof(CollectionStepCompletedEvent),
                createdAt,
                ToJson(new { eventArgs.XPath, eventArgs.ReviewResult }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(PersonalDetailsUpdatedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var data = new { eventArgs.Changes };

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.UserId,
                EntryType.PersonalDetails,
                nameof(PersonalDetailsUpdatedEvent),
                createdAt,
                ToJson(data),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(VerificationDetailsUpdatedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var data = new { eventArgs.Changes };

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.UserId,
                EntryType.VerificationDetails,
                nameof(VerificationDetailsUpdatedEvent),
                createdAt,
                ToJson(data),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(TaskCreatedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.TaskId,
                EntryType.Task,
                nameof(TaskCreatedEvent),
                createdAt,
                ToJson(new { eventArgs.VariantId, eventArgs.Type }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(TaskCompletedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.Id,
                EntryType.Task,
                nameof(TaskCompletedEvent),
                createdAt,
                ToJson(new { eventArgs.VariantId, eventArgs.Result }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(TaskIncompleteEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.Id,
                EntryType.Task,
                nameof(TaskIncompleteEvent),
                createdAt,
                ToJson(new { eventArgs.VariantId, eventArgs.PreviousResult }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(TaskCollectionStepAddedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.TaskId,
                EntryType.Task,
                nameof(TaskCollectionStepAddedEvent),
                createdAt,
                ToJson(new { eventArgs.CollectionStepId }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        public AuditEntryDto Map(DocumentSubmittedEventArgs eventArgs, DateTime createdAt)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return AuditEntryDto.Create(
                eventArgs.UserId,
                eventArgs.DocumentId,
                EntryType.Document,
                nameof(DocumentSubmittedEvent),
                createdAt,
                ToJson(new { eventArgs.FilesIds }),
                _initiationMapper.Map(eventArgs.Initiation));
        }

        private string ToJson<T>(T data)
        {
            if (data == null)
                return null;

            return _dataSerializer.Serialize(data);
        }
    }
}