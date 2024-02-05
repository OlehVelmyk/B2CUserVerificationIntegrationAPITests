using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Triggers;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface ITriggerRepository
    {
        Task<Trigger> FindNotFiredAsync(Guid triggerVariantId, Guid applicationId);
        
        Task<Trigger> GetAsync(Guid triggerId);

        Task SaveAsync(Trigger trigger);
    }
}