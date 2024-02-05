using System;

namespace WX.B2C.User.Verification.Core.Contracts.Commands
{
    public class AddTaskCommand
    {
        public Guid ApplicationId { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public bool AddCompleted { get; set; }
    }
}