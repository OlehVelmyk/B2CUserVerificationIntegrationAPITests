using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public class CheckVariantInfo
    {
        public Guid Id { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }

        public int? MaxAttempts { get; set; }
    }
}