using System;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers.Configs
{
    public class AddTaskCommandConfig : CommandConfig
    {
        public Guid VariantId { get; set; }

        public bool AddCompleted { get; set; } 
    }
}