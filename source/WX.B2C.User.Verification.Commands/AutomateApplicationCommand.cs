using System;

namespace WX.B2C.User.Verification.Commands
{
    public class AutomateApplicationCommand : VerificationCommand
    {
        public AutomateApplicationCommand(Guid userId, Guid applicationId, string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = userId;
            UserId = userId;
            ApplicationId = applicationId;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public string Reason { get; set; }
    }
}
