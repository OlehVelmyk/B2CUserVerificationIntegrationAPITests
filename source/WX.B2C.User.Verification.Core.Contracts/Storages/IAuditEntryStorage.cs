using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IAuditEntryStorage
    {
        Task<PagedDto<AuditEntryDto>> FindAsync(Guid userId, ODataQueryContext context);
    }
}