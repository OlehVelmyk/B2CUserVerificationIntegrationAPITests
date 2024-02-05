using System;

namespace WX.B2C.User.Verification.Commands
{
    public class CreateExternalProfileCommand : VerificationCommand
    {
        public CreateExternalProfileCommand(Guid userId, string type)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = userId;
            UserId = userId;
            Type = type;
        }

        public Guid UserId { get; set; }

        public string Type { get; set; }
    }
}
