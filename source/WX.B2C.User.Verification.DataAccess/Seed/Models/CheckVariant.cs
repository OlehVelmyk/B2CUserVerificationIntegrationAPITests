using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class CheckVariant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }

        public object Config { get; set; }

        public CheckResult FailResult { get; set; }

        public CheckRunPolicy RunPolicy { get; set; }
    }

    public class CheckRunPolicy
    {
        public int? MaxAttempts { get; set; }
    }
}