using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TaskIncompleteEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public TaskType Type { get; set; }

        public InitiationDto Initiation { get; set; }

        public TaskResult? PreviousResult { get; set; }

        public static TaskIncompleteEventArgs Create(Guid taskId,
                                                      Guid taskVariantId,
                                                      Guid userId,
                                                      TaskType type,
                                                      TaskResult? previousResult,
                                                      InitiationDto initiationDto)
        {
            return new TaskIncompleteEventArgs
            {
                Id = taskId,
                VariantId = taskVariantId,
                UserId = userId,
                Type = type,
                PreviousResult = previousResult,
                Initiation = initiationDto,
            };
        }
    }
}