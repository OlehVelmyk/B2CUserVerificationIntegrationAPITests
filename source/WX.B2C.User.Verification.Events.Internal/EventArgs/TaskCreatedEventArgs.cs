using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TaskCreatedEventArgs : System.EventArgs
    {
        public Guid TaskId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid VariantId { get; set; }
        
        public TaskType Type { get; set; }
        
        public InitiationDto Initiation { get; set; }

        public static TaskCreatedEventArgs Create(Guid taskId, Guid variantId, Guid userId, TaskType type, InitiationDto initiation) =>
            new TaskCreatedEventArgs
            {
                TaskId = taskId,
                UserId = userId,
                VariantId = variantId,
                Type = type,
                Initiation = initiation
            };
    }
}