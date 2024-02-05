using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class PolicyCheckVariant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }

        public string Config { get; set; }

        public CheckResultType? FailResultType { get; set; }

        public string FailResult { get; set; }

        public string FailResultCondition { get; set; }

        public CheckRunPolicy RunPolicy { get; set; }
    }

    public class CheckRunPolicy
    {
        public int? MaxAttempts { get; set; }
    }
}