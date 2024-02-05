using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface ITriggerStorage
    {
        Task<TriggerDto> GetAsync(Guid triggerId);

        Task<TriggerDto[]> GetAllAsync(Guid applicationId);

        Task<Dictionary<string,object>> FindLastContextAsync(Guid variantId, Guid applicationId);
    }
}