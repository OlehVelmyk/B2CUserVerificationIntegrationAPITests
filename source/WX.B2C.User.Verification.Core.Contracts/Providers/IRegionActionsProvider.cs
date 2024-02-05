using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Contracts.Providers
{
    public interface IRegionActionsProvider
    {
        Task<RegionActionsOption> GetAsync(Guid userId);
    }
}
