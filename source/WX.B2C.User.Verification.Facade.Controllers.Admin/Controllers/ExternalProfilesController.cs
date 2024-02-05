using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/external-profiles")]
    public class ExternalProfilesController : ApiController
    {
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly IExternalProfileMapper _externalProfileMapper;

        public ExternalProfilesController(IExternalProfileStorage externalProfileStorage, IExternalProfileMapper externalProfileMapper)
        {
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _externalProfileMapper = externalProfileMapper ?? throw new ArgumentNullException(nameof(externalProfileMapper));
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
    }
}
