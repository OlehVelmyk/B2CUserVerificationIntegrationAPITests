using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public class CollectionStepReadyForReview : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string XPath { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CollectionStepReadyForReview Create(CollectionStep collectionStep, Initiation initiation) =>
            new()
            {
                Id = collectionStep.Id,
                XPath = collectionStep.XPath,
                UserId = collectionStep.UserId,
                Initiation = initiation
            };
    }
}