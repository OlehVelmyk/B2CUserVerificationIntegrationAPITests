using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class TaskCompleted : DomainEvent
    {
        public Guid UserId { get; private set; }

        public Guid TaskId { get; private set; }

        public TaskType Type { get; private set; }

        public Guid VariantId { get; private set; }

        public TaskResult Result { get; private set; }

        public Initiation Initiation { get; private set; }

        public static TaskCompleted Create(VerificationTask task, TaskResult result, Initiation initiation)
        {
            return new TaskCompleted
            {
                UserId = task.UserId,
                TaskId = task.Id,
                VariantId = task.VariantId,
                Type = task.Type,
                Result = result,
                Initiation = initiation
            };
        }
    }
}
