using System;

namespace WX.B2C.User.Verification.Commands
{
    public class ApproveApplicationCommand : VerificationCommand
    {
        public ApproveApplicationCommand(Guid userId, Guid applicationId, bool approveManually, string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = applicationId;
            ApplicationId = applicationId;
            UserId = userId;
            ApproveManually = approveManually;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public bool ApproveManually { get; set; }

        public string Reason { get; set; }
    }
}