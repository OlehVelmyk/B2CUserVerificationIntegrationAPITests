using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOnfidoTokenProvider _onfidoTokenProvider;
        private readonly IExternalProfileProvider _externalProfileProvider;

        public TokenService(IOnfidoTokenProvider onfidoTokenProvider,
                            IExternalProfileProvider externalProfileProvider)
        {
            _onfidoTokenProvider = onfidoTokenProvider ?? throw new ArgumentNullException(nameof(onfidoTokenProvider));
            _externalProfileProvider = externalProfileProvider ?? throw new ArgumentNullException(nameof(externalProfileProvider));
        }

        public async Task<ProviderSdkTokenDto> CreateAsync(Guid userId, CreateTokenDto createTokenDto)
        {
            if (createTokenDto == null)
                throw new ArgumentNullException(nameof(createTokenDto));

            var externalProfile = await _externalProfileProvider.GetOrCreateAsync(userId, ExternalProviderType.Onfido);
            var token = createTokenDto.Type switch
            {
                TokenType.Web         => await _onfidoTokenProvider.CreateWebTokenAsync(externalProfile.Id),
                TokenType.Application => await _onfidoTokenProvider.CreateMobileTokenAsync(externalProfile.Id, createTokenDto.ApplicationId),
                _                     => throw new ArgumentOutOfRangeException(nameof(createTokenDto.Type))
            };

            return new ProviderSdkTokenDto { ApplicantId = externalProfile.Id, Token = token };
        }
    }
}