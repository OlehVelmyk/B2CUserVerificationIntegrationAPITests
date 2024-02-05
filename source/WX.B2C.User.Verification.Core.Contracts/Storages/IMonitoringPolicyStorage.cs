using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IMonitoringPolicyStorage
    {
        Task<MonitoringPolicyDto> FindAsync(MonitoringPolicySelectionContext selectionContext);
    }
}