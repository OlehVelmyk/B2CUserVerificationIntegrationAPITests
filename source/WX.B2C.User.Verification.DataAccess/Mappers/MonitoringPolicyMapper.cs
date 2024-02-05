using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IMonitoringPolicyMapper
    {
        MonitoringPolicyDto Map(MonitoringPolicy policy);
    }

    internal class MonitoringPolicyMapper : IMonitoringPolicyMapper
    {
        public MonitoringPolicyDto Map(MonitoringPolicy policy)
        {
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));

            return new()
            {
                Id = policy.Id,
                Name = policy.Name
            };
        }
    }
}