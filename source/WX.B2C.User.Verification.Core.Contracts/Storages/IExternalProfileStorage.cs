using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IExternalProfileStorage
    {
        Task<ExternalProfileDto[]> FindAsync(Guid userId);

        Task<ExternalProfileDto> FindAsync(Guid userId, ExternalProviderType externalProviderType);

        Task<string> GetExternalIdAsync(Guid userId, ExternalProviderType externalProviderType);

        Task<string> FindExternalIdAsync(Guid userId, ExternalProviderType externalProviderType);
    }
}
