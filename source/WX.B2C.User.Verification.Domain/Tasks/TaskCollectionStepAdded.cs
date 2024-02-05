using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class TaskCollectionStepAdded : DomainEvent
    {
        public Guid TaskId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid CollectionStepId { get; private set; }

        public bool IsRequired { get; private set; }

        public Initiation Initiation { get; private set; }

        public static TaskCollectionStepAdded Create(Guid taskId, Guid userId, Guid collectionStepId, bool isRequired, Initiation initiation) =>
            new() { TaskId = taskId, UserId = userId, CollectionStepId = collectionStepId, IsRequired = isRequired, Initiation = initiation };
    }
}