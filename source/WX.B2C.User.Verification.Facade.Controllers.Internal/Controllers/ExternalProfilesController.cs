using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Internal.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/{userId}/external-profiles")]
    public class ExternalProfilesController : ApiController
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly IExternalProfileProvider _externalProfileProvider;
        private readonly IExternalProfileMapper _externalProfileMapper;

        public ExternalProfilesController(IProfileStorage profileStorage,
                                          IExternalProfileStorage externalProfileStorage,
                                          IExternalProfileProvider extenderProviderService,
                                          IExternalProfileMapper externalProfileMapper)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _externalProfileMapper = externalProfileMapper ?? throw new ArgumentNullException(nameof(externalProfileMapper));
            _externalProfileProvider = extenderProviderService ?? throw new ArgumentNullException(nameof(extenderProviderService));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExternalProfileDto[]>> GetAsync(Guid userId)
        {
            var externalProfile = await _externalProfileStorage.FindAsync(userId);
            var response = externalProfile.Select(_externalProfileMapper.Map);
            return Ok(response);
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExternalProfileDto>> PostAsync([FromRoute] Guid userId, 
                                                                      [FromBody] ExternalProviderType providerType)
        {
            var personalDetails = await _profileStorage.FindPersonalDetailsAsync(userId);
            if (personalDetails == null)
                return ProfileNotFound(userId);

            var externalProfile = await _externalProfileProvider.GetOrCreateAsync(userId, providerType);
            var response = _externalProfileMapper.Map(externalProfile);
            return Ok(response);
        }
    }
}
