using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class TaskCreated : DomainEvent
    {
        public Guid TaskId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid VariantId { get; private set; }

        public TaskType Type { get; private set; }

        public Initiation Initiation { get; private set; }

        public static TaskCreated Create(VerificationTask task, Initiation initiation)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return new TaskCreated
            {
                TaskId = task.Id,
                UserId = task.UserId,
                VariantId = task.VariantId,
                Type = task.Type,
                Initiation = initiation
            };
        }
    }
}
