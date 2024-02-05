using System;

namespace WX.B2C.User.Verification.Commands
{
    public class SubmitCollectionStepCommand : VerificationCommand
    {
        public SubmitCollectionStepCommand(Guid stepId, string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = stepId;

            StepId = stepId;
            Reason = reason;
        }

        public Guid StepId { get; set; }

        public string Reason { get; set; }
    }
}
