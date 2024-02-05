using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface ICheckStorage
    {
        Task<CheckDto[]> FindByExternalIdAsync(string externalId, CheckProviderType provider);

        Task<CheckDto[]> GetAllAsync(Guid userId);

        Task<CheckDto[]> GetPendingAsync(Guid userId);

        Task<CheckDto> GetAsync(Guid checkId);

        Task<CheckDto> FindAsync(Guid checkId, Guid userId);

        Task<CheckDto[]> GetAsync(Guid userId , Guid[] variantIds);

        Task<Guid[]> GetRelatedTasksAsync(Guid checkId);
    }
}
