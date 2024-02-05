using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using D = WX.B2C.User.Verification.Domain;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/applications")]
    public class ApplicationsController : ApiController
    {
        private readonly IApplicationService _applicationService;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationAggregationService _aggregationService;
        private readonly IInitiationProvider _initiationProvider;

        public ApplicationsController(IApplicationStorage applicationStorage,
                                      IApplicationService applicationService,
                                      IApplicationAggregationService aggregationService,
                                      IInitiationProvider initiationProvider)
        {
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDto[]>> GetAllAsync([FromRoute] Guid userId)
        {
            var applications = await _applicationStorage.FindAsync(userId);
            var response = await _aggregationService.AggregateAsync(applications);
            return Ok(response);
        }

        [HttpGet("{applicationId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDto>> GetAsync([FromRoute] Guid userId, [FromRoute] Guid applicationId)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var response = await _aggregationService.AggregateAsync(application);
            return Ok(response);
        }

        [HttpGet("default")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDto>> GetDefaultAsync([FromRoute] Guid userId)
        {
            var productType = D.Models.ProductType.WirexBasic;
            var application = await _applicationStorage.FindAsync(userId, productType);
            if (application == null)
                return ApplicationNotFound(userId, productType);

            var response = await _aggregationService.AggregateAsync(application);
            return Ok(response);
        }

        [HttpPost("{applicationId}/approve")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ApproveAsync([FromRoute] Guid userId,
                                                      [FromRoute] Guid applicationId,
                                                      [FromBody] ReasonDto reasonDto)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _applicationService.ApproveAsync(applicationId, initiation);
            return NoContent();
        }

        [HttpPost("{applicationId}/reject")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectAsync([FromRoute] Guid userId,
                                                     [FromRoute] Guid applicationId,
                                                     [FromBody] ReasonDto reasonDto)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _applicationService.RejectAsync(applicationId, initiation);
            return NoContent();
        }

        [HttpPost("{applicationId}/review")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RequestReviewAsync([FromRoute] Guid userId,
                                                            [FromRoute] Guid applicationId,
                                                            [FromBody] ReasonDto reasonDto)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _applicationService.RequestReviewAsync(applicationId, initiation);
            return NoContent();
        }

        [HttpPost("{applicationId}/revert")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevertDecisionAsync([FromRoute] Guid userId,
                                                             [FromRoute] Guid applicationId,
                                                             [FromBody] ReasonDto reasonDto)
        {
            var application = await _applicationStorage.FindAsync(userId, applicationId);
            if (application == null)
                return ApplicationNotFound(userId, applicationId);

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _applicationService.RevertDecisionAsync(applicationId, initiation);
            return NoContent();
        }
    }
}