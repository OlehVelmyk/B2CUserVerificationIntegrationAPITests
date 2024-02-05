using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Internal.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/providers")]
    public class ProvidersController : ApiController
    {
        private readonly ITokenService _tokenService;
        private readonly ISdkTokenMapper _tokenMapper;
        private readonly IProfileStorage _profileStorage;

        public ProvidersController(ITokenService tokenService, ISdkTokenMapper tokenMapper, IProfileStorage profileStorage)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _tokenMapper = tokenMapper ?? throw new ArgumentNullException(nameof(tokenMapper));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
        }

        [HttpPost("onfido/sdk-token")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SdkTokenDto>> PostAsync([FromBody] SdkTokenRequest request)
        {
            var userId = request.UserId;
            var personalDetails = await _profileStorage.FindPersonalDetailsAsync(userId);
            if (personalDetails == null)
                return ProfileNotFound(userId);

            var dto = _tokenMapper.Map(TokenProvider.Onfido, request);
            var providerTokenDto = await _tokenService.CreateAsync(userId, dto);
            return Ok(_tokenMapper.Map(providerTokenDto));
        }
    }
}