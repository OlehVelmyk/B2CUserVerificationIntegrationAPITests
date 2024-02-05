using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TaskCompletedEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }
        
        public TaskType Type { get; set; }

        public InitiationDto Initiation { get; set; }

        public TaskResult Result { get; set; }

        public static TaskCompletedEventArgs Create(Guid taskId,
                                                    Guid variantId,
                                                    Guid userId,
                                                    TaskType type,
                                                    TaskResult result,
                                                    InitiationDto initiation) =>
            new TaskCompletedEventArgs
            {
                Id = taskId,
                VariantId = variantId,
                UserId = userId,
                Type = type,
                Result = result,
                Initiation = initiation,
            };
    }
}
