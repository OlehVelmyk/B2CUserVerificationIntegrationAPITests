using System;
using System.Linq;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.Messaging.Core;
using CollectionStepReviewResult = WX.B2C.User.Verification.Events.Internal.Enums.CollectionStepReviewResult;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal class InnerEventMapper : IInnerEventMapper
    {
        private readonly IOperationContextProvider _operationContextProvider;

        public InnerEventMapper(IOperationContextProvider operationContextProvider)
        {
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
        }

        private Guid CausationId => _operationContextProvider.GetContextOrDefault().OperationId;

        private Guid CorrelationId => _operationContextProvider.GetContextOrDefault().CorrelationId;

        public Event Map(VerificationDetailsUpdated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = VerificationDetailsUpdatedEventArgs.Create(domainEvent.UserId,
                                                                  domainEvent.Changes.Select(Map).ToArray(),
                                                                  Map(domainEvent.Initiation));

            return new VerificationDetailsUpdatedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(ApplicationRequiredTaskAdded domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = ApplicationRequiredTaskAddedEventArgs.Create(domainEvent.ApplicationId,
                                                                    domainEvent.UserId,
                                                                    domainEvent.TaskId,
                                                                    Map(domainEvent.Initiation));

            return new ApplicationRequiredTaskAddedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }


        public Event Map(ApplicationAutomated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new ApplicationAutomatedEventArgs
            {
                ApplicationId = domainEvent.ApplicationId,
                UserId = domainEvent.UserId,
                Initiation = Map(domainEvent.Initiation)
            };

            return new ApplicationAutomatedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CollectionStepUpdated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CollectionStepUpdatedEventArgs.Create(
                domainEvent.Id, 
                domainEvent.UserId, 
                domainEvent.IsRequired,
                domainEvent.IsReviewNeeded, 
                Map(domainEvent.Initiation));

            return new CollectionStepUpdatedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(PersonalDetailsUpdated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = PersonalDetailsUpdatedEventArgs.Create(domainEvent.UserId,
                                                              domainEvent.Changes.Select(Map).ToArray(),
                                                              Map(domainEvent.Initiation));

            return new PersonalDetailsUpdatedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(DocumentSubmitted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = DocumentSubmittedEventArgs.Create(domainEvent.DocumentId,
                                                         domainEvent.UserId,
                                                         domainEvent.FilesIds,
                                                         (DocumentCategory)domainEvent.DocumentCategory,
                                                         domainEvent.DocumentType.ToString(),
                                                         Map(domainEvent.Initiation));

            return new DocumentSubmittedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(ApplicationStateChanged domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = ApplicationStateChangedEventArgs.Create(domainEvent.UserId,
                                                               domainEvent.ApplicationId,
                                                               (ApplicationState)domainEvent.PreviousState,
                                                               (ApplicationState)domainEvent.NewState,
                                                               domainEvent.DecisionReasons,
                                                               Map(domainEvent.Initiation));

            return new ApplicationStateChangedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckCreated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckCreatedEventArgs.Create(domainEvent.Id,
                                                    domainEvent.UserId,
                                                    domainEvent.VariantId,
                                                    domainEvent.RelatedTasks,
                                                    Map(domainEvent.Initiation));

            return new CheckCreatedEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckStarted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckStartedEventArgs.Create(
                domainEvent.Id,
                domainEvent.UserId,
                domainEvent.VariantId);

            return new CheckStartedEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckPerformed domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckPerformedEventArgs.Create(
                domainEvent.Id,
                domainEvent.UserId,
                domainEvent.VariantId);

            return new CheckPerformedEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckCompletedEventArgs.Create(domainEvent.Id,
                                                      domainEvent.UserId,
                                                      domainEvent.VariantId,
                                                      domainEvent.Type.To<CheckType>(),
                                                      domainEvent.Provider.To<CheckProviderType>(),
                                                      domainEvent.Result.To<CheckResult>(),
                                                      domainEvent.Decision);

            return new CheckCompletedEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckErrorOccurred domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckErrorOccuredEventArgs.Create(domainEvent.Id,
                                                         domainEvent.UserId,
                                                         domainEvent.VariantId,
                                                         domainEvent.Type.To<CheckType>());


            return new CheckErrorOccuredEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CollectionStepCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CollectionStepCompletedEventArgs.Create(domainEvent.Id,
                                                               domainEvent.UserId,
                                                               domainEvent.XPath,
                                                               domainEvent.ReviewResult.As<CollectionStepReviewResult>(),
                                                               Map(domainEvent.Initiation));

            return new CollectionStepCompletedEvent(args.CollectionStepId.ToString(), args, CausationId, CorrelationId);
        }
        public Event Map(CollectionStepRequested domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CollectionStepRequestedEventArgs.Create(domainEvent.Id,
                                                               domainEvent.UserId,
                                                               domainEvent.XPath,
                                                               domainEvent.IsRequired,
                                                               domainEvent.IsReviewNeeded,
                                                               Map(domainEvent.Initiation));

            return new CollectionStepRequestedEvent(args.CollectionStepId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CollectionStepRequired domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CollectionStepRequiredEventArgs.Create(domainEvent.Id,
                                                              domainEvent.UserId,
                                                              Map(domainEvent.Initiation));

            return new CollectionStepRequiredEvent(args.CollectionStepId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CollectionStepReadyForReview domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CollectionStepReadyForReviewEventArgs.Create(domainEvent.Id,
                                                                    domainEvent.UserId,
                                                                    domainEvent.XPath,
                                                                    Map(domainEvent.Initiation));

            return new CollectionStepReadyForReviewEvent(args.CollectionStepId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TaskCreated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = TaskCreatedEventArgs.Create(domainEvent.TaskId, domainEvent.VariantId,
                                                   domainEvent.UserId, (TaskType)domainEvent.Type,
                                                   Map(domainEvent.Initiation));

            return new TaskCreatedEvent(args.TaskId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TaskCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = TaskCompletedEventArgs.Create(domainEvent.TaskId, domainEvent.VariantId,
                                                     domainEvent.UserId, (TaskType)domainEvent.Type,
                                                     (TaskResult)domainEvent.Result, Map(domainEvent.Initiation));

            return new TaskCompletedEvent(args.Id.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TaskCollectionStepAdded domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = TaskCollectionStepAddedEventArgs.Create(domainEvent.TaskId,
                                                               domainEvent.UserId,
                                                               domainEvent.CollectionStepId,
                                                               domainEvent.IsRequired, 
                                                               Map(domainEvent.Initiation));

            return new TaskCollectionStepAddedEvent(args.TaskId.ToString(), args, CausationId, CorrelationId);
        }


        public Event Map(TaskCollectionStepRemoved domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = TaskCollectionStepRemovedEventArgs.Create(domainEvent.TaskId, domainEvent.UserId,
                                                                 domainEvent.CollectionStepsIds);

            return new TaskCollectionStepRemovedEvent(args.TaskId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TaskIncomplete domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = TaskIncompleteEventArgs.Create(domainEvent.TaskId, domainEvent.TaskVariantId,
                                                       domainEvent.UserId, (TaskType)domainEvent.Type,
                                                       (TaskResult?)domainEvent.PreviousResult,
                                                       Map(domainEvent.Initiation));

            return new TaskIncompleteEvent(domainEvent.TaskId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TriggerFired domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new TriggerFiredEventArgs
            {
                UserId = domainEvent.UserId,
                ApplicationId = domainEvent.ApplicationId,
                FiringDate = domainEvent.FiringDate,
                TriggerId = domainEvent.TriggerId,
                VariantId = domainEvent.VariantId,
            };

            return new TriggerFiredEvent(domainEvent.TriggerId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TriggerScheduled domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new TriggerScheduledEventArgs
            {
                UserId = domainEvent.UserId,
                ApplicationId = domainEvent.ApplicationId,
                ScheduleDate = domainEvent.ScheduleDate,
                TriggerId = domainEvent.TriggerId,
                VariantId = domainEvent.VariantId,
            };

            return new TriggerScheduledEvent(domainEvent.TriggerId.ToString(), args, CausationId, CorrelationId);
        }
        
        public Event Map(TriggerUnscheduled domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new TriggerUnscheduledEventArgs
            {
                UserId = domainEvent.UserId,
                ApplicationId = domainEvent.ApplicationId,
                UnscheduleDate = domainEvent.UnscheduleDate,
                TriggerId = domainEvent.TriggerId,
                VariantId = domainEvent.VariantId,
            };

            return new TriggerUnscheduledEvent(domainEvent.TriggerId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(TriggerCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new TriggerCompletedEventArgs
            {
                UserId = domainEvent.UserId,
                ApplicationId = domainEvent.ApplicationId,
                TriggerId = domainEvent.TriggerId,
                VariantId = domainEvent.VariantId,
            };

            return new TriggerCompletedEvent(domainEvent.TriggerId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(ScheduledTriggerJobFinished domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new ScheduledTriggerJobFinishedArgs
            {
                UserId = domainEvent.UserId,
                TriggerId = domainEvent.TriggerId,
                FinishingDate = domainEvent.FinishingDate
            };

            return new ScheduledTriggerJobFinishedEvent(domainEvent.TriggerId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(ExternalProfileCreated domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = ExternalProfileCreatedEventArgs.Create(domainEvent.UserId,
                                                              domainEvent.ProviderType.To<ExternalProviderType>());

            return new ExternalProfileCreatedEvent(domainEvent.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(UserTriggersActionRequired domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = new UserTriggersActionRequiredEventArgs
            {
                UserId = domainEvent.UserId,
                Actions = domainEvent.TriggerActions.ToArray(),
                TriggerPolicyId = domainEvent.TriggerPolicyId
            };

            return new UserTriggersActionRequiredEvent(domainEvent.UserId.ToString(), args, CausationId, CorrelationId);
        }
        
        public Event Map(UserReminderJobFinished domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = UserReminderJobFinishedEventArgs.Create(domainEvent.Id, domainEvent.UserId);

            return new UserReminderJobFinishedEvent(domainEvent.Id.ToString(), args, CausationId, CorrelationId);
        }

        private static InitiationDto Map(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new InitiationDto
            {
                Initiator = initiation.Initiator,
                Reason = initiation.Reason,
            };
        }

        private static PropertyChangeDto Map(PropertyChange change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));

            var (propertyName, newValue, oldValue) = change;
            return new PropertyChangeDto
            {
                PropertyName = propertyName,
                NewValue = PropertyChangeSerializer.Serialize(newValue),
                PreviousValue = PropertyChangeSerializer.Serialize(oldValue),
                IsReset = change.IsReset
            };
        }
    }
}