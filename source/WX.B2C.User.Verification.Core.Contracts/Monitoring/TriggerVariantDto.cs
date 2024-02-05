using System;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Triggers.Configs;

namespace WX.B2C.User.Verification.Core.Contracts.Monitoring
{
    public class TriggerVariantDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Can be onboarding or monitoring policy
        /// </summary>
        public Guid PolicyId { get; set; }

        public string Name { get; set; }

        public bool Iterative { set; get; }

        public Condition[] Preconditions { get; set; }

        public Condition[] Conditions { get; set; }

        public CommandConfig[] Commands { get; set; }

        public Schedule Schedule { get; set; }

        public bool IsScheduled => Schedule != null;

        public bool HasPreconditions => Preconditions is { Length: > 0 };

        public bool HasConditions => Conditions is { Length: > 0 };
    }
}