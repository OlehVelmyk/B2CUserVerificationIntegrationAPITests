using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class ValidationPolicy
    {
        public Guid Id { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public List<PolicyValidationRule> Rules { get; set; }
    }
}