using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class ValidationPolicy
    {
        public Guid Id { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public Guid[] Rules { get; set; }
    }
}