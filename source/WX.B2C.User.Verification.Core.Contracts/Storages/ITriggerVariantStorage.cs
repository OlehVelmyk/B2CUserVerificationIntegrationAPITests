using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface ITriggerVariantStorage
    {
        Task<TriggerVariantDto[]> FindAsync(Guid policyId);

        Task<TriggerVariantDto> GetAsync(Guid triggerVariantId);
    }
}