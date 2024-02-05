using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public class CollectionStepRequested : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string XPath { get; private set; }

        public bool IsRequired { get; private set; }

        public bool IsReviewNeeded { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CollectionStepRequested Create(CollectionStep collectionStep, Initiation initiation)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new CollectionStepRequested
            {
                Id = collectionStep.Id,
                UserId = collectionStep.UserId,
                Initiation = initiation,
                XPath = collectionStep.XPath,
                IsRequired = collectionStep.IsRequired,
                IsReviewNeeded = collectionStep.IsReviewNeeded
            };
        }
    }
}