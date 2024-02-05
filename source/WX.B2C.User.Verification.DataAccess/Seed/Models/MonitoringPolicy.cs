using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class MonitoringPolicy
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public Trigger[] Triggers { get; set; }

        public Task[] Templates { get; set; }
    }
}