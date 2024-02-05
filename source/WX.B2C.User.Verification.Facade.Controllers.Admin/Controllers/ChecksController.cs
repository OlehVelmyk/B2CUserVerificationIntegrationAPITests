using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/checks")]
    public class ChecksController : ApiController
    {
        private readonly ICheckStorage _checkStorage;
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ICheckService _checkService;
        private readonly ICheckAggregationService _aggregationService;
        private readonly IInitiationProvider _initiationProvider;

        public ChecksController(ICheckStorage checkStorage,
                                IVerificationPolicyStorage policyStorage,
                                ICheckService checkService,
                                ICheckAggregationService aggregationService,
                                IInitiationProvider initiationProvider)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckDto[]>> GetAllAsync([FromRoute] Guid userId)
        {
            var checks = await _checkStorage.GetAllAsync(userId);
            var response = await _aggregationService.AggregateAsync(checks);
            return Ok(response);
        }

        [HttpGet("{checkId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckDto>> GetAsync([FromRoute] Guid userId,
                                                           [FromRoute] Guid checkId)
        {
            var check = await _checkStorage.FindAsync(checkId, userId);
            if (check == null)
                return CheckNotFound();

            var response = await _aggregationService.AggregateAsync(check);
            return Ok(response);
        }

        [HttpPost("")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RequestAsync([FromRoute] Guid userId,
                                                      [FromBody] CheckRequest request)
        {
            var checkVariant = await _policyStorage.FindCheckInfoAsync(request.VariantId);
            if (checkVariant == null)
                return CheckVariantNotFound();
            
            var checkRequest = new Core.Contracts.Dtos.NewCheckDto
            {
                CheckType = checkVariant.Type,
                Provider = checkVariant.Provider,
                VariantId = checkVariant.Id,
                RelatedTasks = request.RelatedTasks
            };
            var initiation = _initiationProvider.Create(request.Reason);

            await _checkService.RequestAsync(
                userId,
                checkRequest,
                initiation);

            return NoContent();
        }
    }
}