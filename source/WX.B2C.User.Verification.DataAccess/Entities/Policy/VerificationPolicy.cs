using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class VerificationPolicy
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public ICollection<PolicyTask> Tasks { get; set; }

        public RejectionPolicy RejectionPolicy { get; set; }
    }
}