using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    public interface ICollectionStepRepository
    {
        Task<CollectionStep> FindNotCompletedAsync(Guid userId, string xPath);

        Task<CollectionStep> GetAsync(Guid id);

        Task SaveAsync(CollectionStep step);

        Task RemoveAsync(Guid id);
    }
}
