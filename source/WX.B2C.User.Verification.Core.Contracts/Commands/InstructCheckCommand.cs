using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Commands
{
    public class InstructCheckCommand
    {
        public Guid ApplicationId { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public TaskType TaskType { get; set; }
        
        public bool Force { get; set; }
    }
}