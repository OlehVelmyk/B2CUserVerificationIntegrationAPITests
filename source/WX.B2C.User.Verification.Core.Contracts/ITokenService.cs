using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ITokenService
    {
        Task<ProviderSdkTokenDto> CreateAsync(Guid userId, CreateTokenDto createTokenDto);
    }
}