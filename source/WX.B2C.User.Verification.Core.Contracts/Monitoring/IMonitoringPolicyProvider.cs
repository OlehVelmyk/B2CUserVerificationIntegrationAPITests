using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Core.Contracts.Monitoring
{
    public interface IMonitoringPolicyProvider
    {
        Task<MonitoringPolicyDto> FindAsync(Guid userId);
    }
}