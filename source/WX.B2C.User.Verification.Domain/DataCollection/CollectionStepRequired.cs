using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public class CollectionStepRequired : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CollectionStepRequired Create(CollectionStep collectionStep, Initiation initiation)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new CollectionStepRequired
            {
                Id = collectionStep.Id,
                UserId = collectionStep.UserId,
                Initiation = initiation,
            };
        }
    }
}