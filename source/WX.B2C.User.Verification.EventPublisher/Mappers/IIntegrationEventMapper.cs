using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal interface IIntegrationEventMapper
    {
        Event Map(ApplicationRegistered domainEvent);

        Event Map(VerificationDetailsUpdated domainEvent);

        Event Map(ApplicationStateChanged domainEvent);

        Event Map(CheckCompleted domainEvent);

        Event Map(CollectionStepCompleted domainEvent);

        Event Map(CollectionStepReadyForReview domainEvent);

        Event Map(CollectionStepRequested domainEvent);
    }
}
