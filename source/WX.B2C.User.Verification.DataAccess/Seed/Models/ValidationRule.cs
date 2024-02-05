using System;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class ValidationRule
    {
        public Guid Id { get; set; }

        public string RuleType { get; set; }

        public string RuleSubject { get; set; }

        public object Validations { get; set; }
    }
}
