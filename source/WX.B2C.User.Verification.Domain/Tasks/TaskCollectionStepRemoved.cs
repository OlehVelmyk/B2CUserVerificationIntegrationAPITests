using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class TaskCollectionStepRemoved : DomainEvent
    {
        public Guid TaskId { get; private set; }
        
        public Guid UserId { get; private set; }
        
        public Guid[] CollectionStepsIds { get; private set; }

        public static TaskCollectionStepRemoved Create(Guid taskId, Guid userId, Guid[] collectionStepsIds) =>
            new() { TaskId = taskId, UserId = userId, CollectionStepsIds = collectionStepsIds };
    }
}