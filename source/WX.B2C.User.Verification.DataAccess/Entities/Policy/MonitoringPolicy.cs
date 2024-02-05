using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class MonitoringPolicy
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }
    }
}