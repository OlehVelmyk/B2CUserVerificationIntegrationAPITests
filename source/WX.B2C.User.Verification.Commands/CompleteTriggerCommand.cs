using System;

namespace WX.B2C.User.Verification.Commands
{
    public class CompleteTriggerCommand : VerificationCommand
    {
        public CompleteTriggerCommand(Guid triggerId)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = TriggerId = triggerId;
        }

        public Guid TriggerId { get; set; }
    }
}
