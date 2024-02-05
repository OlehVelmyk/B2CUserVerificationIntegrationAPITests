using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IAuditEntryRepository
    {
        Task SaveAsync(AuditEntryDto auditDto);
    }
}