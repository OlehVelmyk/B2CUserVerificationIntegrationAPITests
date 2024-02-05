using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal interface IInnerEventMapper
    {
        Event Map(VerificationDetailsUpdated domainEvent);

        Event Map(ApplicationRequiredTaskAdded domainEvent);

        Event Map(PersonalDetailsUpdated domainEvent);

        Event Map(DocumentSubmitted domainEvent);

        Event Map(ApplicationStateChanged domainEvent);

        Event Map(CheckCreated domainEvent);

        Event Map(CheckStarted domainEvent);

        Event Map(CheckPerformed domainEvent);

        Event Map(CheckCompleted domainEvent);

        Event Map(CheckErrorOccurred domainEvent);

        Event Map(CollectionStepCompleted domainEvent);

        Event Map(CollectionStepRequested domainEvent);

        Event Map(CollectionStepRequired domainEvent);

        Event Map(CollectionStepReadyForReview domainEvent);

        Event Map(TaskCreated domainEvent);

        Event Map(TaskCompleted domainEvent);

        Event Map(TaskCollectionStepAdded domainEvent);

        Event Map(TaskCollectionStepRemoved domainEvent);

        Event Map(TaskIncomplete domainEvent);

        Event Map(TriggerScheduled domainEvent);

        Event Map(TriggerFired domainEvent);

        Event Map(TriggerUnscheduled domainEvent);
        
        Event Map(TriggerCompleted domainEvent);

        Event Map(ScheduledTriggerJobFinished domainEvent);

        Event Map(ExternalProfileCreated domainEvent);

        Event Map(UserTriggersActionRequired domainEvent);

        Event Map(ApplicationAutomated domainEvent);

        Event Map(CollectionStepUpdated domainEvent);

        Event Map(UserReminderJobFinished domainEvent);
    }
}