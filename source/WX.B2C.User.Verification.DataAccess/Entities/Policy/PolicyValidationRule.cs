using System;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class PolicyValidationRule
    {
        public Guid ValidationRuleId { get; set; }

        public Guid ValidationPolicyId { get; set; }

        public virtual ValidationRule Rule { get; set; }
    }
}