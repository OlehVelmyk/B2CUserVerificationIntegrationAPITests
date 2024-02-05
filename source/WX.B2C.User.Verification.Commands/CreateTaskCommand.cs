using System;

namespace WX.B2C.User.Verification.Commands
{
    public class CreateTaskCommand : VerificationCommand
    {
        public CreateTaskCommand(Guid userId,
                                 Guid taskId,
                                 string type,
                                 Guid variantId,
                                 Guid[] acceptanceCheckIds,
                                 Guid[] collectionStepIds,
                                 string reason)
        {
            CommandId = taskId;
            CommandChainId = userId;
            UserId = userId;
            VariantId = variantId;
            Type = type;
            AcceptanceCheckIds = acceptanceCheckIds;
            CollectionStepIds = collectionStepIds;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public string Type { get; set; }

        public Guid VariantId { get; set; }

        public Guid[] AcceptanceCheckIds { get; set; }

        public Guid[] CollectionStepIds { get; set; }

        public string Reason { get; set; }
    }
}
