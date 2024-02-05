using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class TaskIncomplete : DomainEvent
    {
        public Guid TaskId { get; private set; }

        public Guid TaskVariantId { get; private set; }

        public Guid UserId { get; private set; }
        
        public TaskType Type { get; private set; }

        public TaskResult? PreviousResult { get; private set; }

        public Initiation Initiation { get; private set; }

        public static TaskIncomplete Create(VerificationTask task,
                                            TaskResult? previousResult,
                                            Initiation initiation) =>
            new()
            {
                TaskId = task.Id,
                TaskVariantId = task.VariantId,
                UserId = task.UserId,
                Type = task.Type,
                PreviousResult = previousResult,
                Initiation = initiation
            };
    }
}