using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public class CollectionStepUpdated : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public bool IsRequired { get; private set; }

        public bool IsReviewNeeded { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CollectionStepUpdated Create(CollectionStep collectionStep, Initiation initiation)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new CollectionStepUpdated
            {
                Id = collectionStep.Id,
                UserId = collectionStep.UserId,
                IsRequired = collectionStep.IsRequired,
                IsReviewNeeded = collectionStep.IsReviewNeeded,
                Initiation = initiation
            };
        }
    }
}
