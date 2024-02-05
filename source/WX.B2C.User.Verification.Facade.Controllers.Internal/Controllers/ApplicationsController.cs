using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/v{version:apiVersion}/verification/{userId}/applications")]
    public class ApplicationsController : ApiController
    {
        private readonly IApplicationStateChangelogStorage _applicationStateChangelogStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationMapper _applicationMapper;

        public ApplicationsController(
            IApplicationStorage applicationStorage,
            IApplicationMapper applicationMapper,
            IApplicationStateChangelogStorage applicationStateChangelogStorage)
        {
            _applicationStateChangelogStorage = applicationStateChangelogStorage ?? throw new ArgumentNullException(nameof(applicationStateChangelogStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _applicationMapper = applicationMapper ?? throw new ArgumentNullException(nameof(applicationMapper));
        }

        [HttpGet("{applicationId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDto>> GetAsync([FromRoute] Guid userId, [FromRoute] Guid applicationId)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var applicationStateChangelog = await _applicationStateChangelogStorage.FindAsync(application.Id);
            var response = _applicationMapper.Map(application, applicationStateChangelog);
            return Ok(response);
        }

        [HttpGet("default")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDto>> GetDefaultAsync([FromRoute] Guid userId)
        {
            const ProductType productType = ProductType.WirexBasic;
            var application = await _applicationStorage.FindAsync(userId, productType);
            if (application == null)
                return ApplicationNotFound(userId, productType);

            var applicationStateChangelog = await _applicationStateChangelogStorage.FindAsync(application.Id);
            var response = _applicationMapper.Map(application, applicationStateChangelog);
            return Ok(response);
        }
    }
}