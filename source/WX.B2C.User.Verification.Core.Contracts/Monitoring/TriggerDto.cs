using System;
using WX.B2C.User.Verification.Domain.Triggers;

namespace WX.B2C.User.Verification.Core.Contracts.Monitoring
{
    public class TriggerDto
    {
        public Guid TriggerId { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public TriggerState State { get; set; }
    }
}