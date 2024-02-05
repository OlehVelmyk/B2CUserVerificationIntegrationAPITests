using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    /// <summary>
    /// Service to save auditable information about Application,Tasks,Checks.
    /// Can read additional information for saving if needed.
    /// </summary>
    public interface IAuditService
    { 
        Task SaveAsync(AuditEntryDto auditDto);
    }
}