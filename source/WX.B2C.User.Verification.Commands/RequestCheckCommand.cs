using System;

namespace WX.B2C.User.Verification.Commands
{
    public class RequestCheckCommand : VerificationCommand
    {
        public RequestCheckCommand(Guid userId,
                                  Guid checkId,
                                  Guid variantId,
                                  string type,
                                  string provider,
                                  Guid[] relatedTasks,
                                  string reason)
        {
            CommandId = checkId;
            CommandChainId = userId;
            UserId = userId;
            VariantId = variantId;
            Type = type;
            Provider = provider;
            RelatedTasks = relatedTasks;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }

        public string Type { get; set; }

        public string Provider { get; set; }

        public Guid[] RelatedTasks { get; set; }

        public string Reason { get; set; }
    }
}
