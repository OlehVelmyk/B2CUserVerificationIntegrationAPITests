using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public class CollectionStepCancelled : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string XPath { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CollectionStepCancelled Create(CollectionStep collectionStep, Initiation initiation)
        {
            return new CollectionStepCancelled
            {
                Id = collectionStep.Id,
                XPath = collectionStep.XPath,
                UserId = collectionStep.UserId,
                Initiation = initiation
            };
        }
    }
}