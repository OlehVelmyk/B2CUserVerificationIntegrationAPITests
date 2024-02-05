using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers.Configs
{
    public class InstructCheckCommandConfig : CommandConfig
    {
        public Guid VariantId { get; set; }
        
        public TaskType TaskType { get; set; }

        public bool Force { get; set; }
    }
}