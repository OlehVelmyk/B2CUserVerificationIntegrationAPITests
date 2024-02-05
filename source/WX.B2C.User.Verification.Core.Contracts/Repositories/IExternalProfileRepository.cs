using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IExternalProfileRepository
    {
        Task SaveAsync(Guid userId, ExternalProfileDto externalProfileDto);        

        Task<ExternalProfileDto> FindAsync(Guid userId, ExternalProviderType providerType);
    }
}