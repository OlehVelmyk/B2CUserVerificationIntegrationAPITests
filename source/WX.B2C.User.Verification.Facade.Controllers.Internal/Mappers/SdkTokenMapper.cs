using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface ISdkTokenMapper
    {
        CreateTokenDto Map(TokenProvider tokenProvider, SdkTokenRequest request);

        SdkTokenDto Map(ProviderSdkTokenDto providerSdkTokenDto);
    }

    internal class SdkTokenMapper : ISdkTokenMapper
    {
        public CreateTokenDto Map(TokenProvider tokenProvider, SdkTokenRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new CreateTokenDto
            {
                Provider = tokenProvider,
                Type = request.Type,
                ApplicationId = request.ApplicationId
            };
        }

        public SdkTokenDto Map(ProviderSdkTokenDto providerSdkTokenDto)
        {
            if (providerSdkTokenDto == null)
                throw new ArgumentNullException(nameof(providerSdkTokenDto));

            return new SdkTokenDto
            {
                Token = providerSdkTokenDto.Token,
                ApplicantId = providerSdkTokenDto.ApplicantId
            };
        }
    }
}