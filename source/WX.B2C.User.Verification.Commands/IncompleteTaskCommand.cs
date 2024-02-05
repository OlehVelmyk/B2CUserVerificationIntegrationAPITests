using System;

namespace WX.B2C.User.Verification.Commands
{
    public class IncompleteTaskCommand : VerificationCommand
    {
        public IncompleteTaskCommand(Guid id, Guid userId, string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = id;
            Id = id;
            UserId = userId;
            Reason = reason;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Reason { get; set; }
    }
}