using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IExternalProfileProvider
    {
        public Task<ExternalProfileDto> GetOrCreateAsync(Guid userId, ExternalProviderType externalProviderType);
    }
}