using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface ICollectionStepStorage
    {
        Task<CollectionStepDto[]> GetAllAsync(Guid userId);

        Task<CollectionStepDto[]> GetAllAsync(Guid userId, string xPath);

        Task<CollectionStepDto> GetAsync(Guid id);

        Task<CollectionStepDto> FindAsync(Guid id, Guid userId);

        Task<CollectionStepDto> FindAsync(Guid userId, string xPath);

        Task<CollectionStepDto[]> FindRequestedAsync(Guid userId, params string[] xPathes);

        Task<CollectionStepDto[]> FindRequestedAsync(Guid userId);

        Task<Guid[]> GetRelatedTasksAsync(Guid checkId);
    }
}