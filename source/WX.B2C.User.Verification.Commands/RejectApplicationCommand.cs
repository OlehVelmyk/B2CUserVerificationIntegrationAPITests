using System;

namespace WX.B2C.User.Verification.Commands
{
    public class RejectApplicationCommand : VerificationCommand
    {
        public RejectApplicationCommand(Guid userId, Guid applicationId, string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = applicationId;
            UserId = userId;
            ApplicationId = applicationId;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public string Reason { get; set; }
    }
}
