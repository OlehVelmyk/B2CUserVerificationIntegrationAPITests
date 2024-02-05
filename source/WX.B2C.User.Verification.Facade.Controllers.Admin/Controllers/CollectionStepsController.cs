using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    //TODO revise collection steps domain logic. For now we have only one rule: one requested step with XPath+UserId. And can have infinite number of completed steps.
    //TODO alternative solution is to remove duplication steps from Task.CollectionSteps and collection step table.
    //TODO Obsolete steps are needed only for History and Audit. So better to move this logic to audit

    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/collection-steps")]
    public class CollectionStepController : ApiController
    {
        private readonly ICollectionStepStorage _stepStorage;
        private readonly ICollectionStepService _service;
        private readonly ITaskService _taskService;
        private readonly ICollectionStepAggregationService _aggregationService;
        private readonly ICollectionStepMapper _mapper;
        private readonly IInitiationProvider _initiationProvider;

        public CollectionStepController(ICollectionStepStorage collectionStepStorage,
                                        ICollectionStepService service,
                                        ITaskService taskService,
                                        ICollectionStepAggregationService aggregationService,
                                        ICollectionStepMapper mapper,
                                        IInitiationProvider initiationProvider)
        {
            _stepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CollectionStepDto[]>> GetAllAsync([FromRoute] Guid userId)
        {
            var collectionSteps = await _stepStorage.GetAllAsync(userId);
            var response = await _aggregationService.AggregateAsync(collectionSteps);
            return Ok(response);
        }

        [HttpGet("{stepId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CollectionStepDto>> GetAsync([FromRoute] Guid userId,
                                                                    [FromRoute] Guid stepId)
        {
            var step = await _stepStorage.FindAsync(stepId, userId);
            if (step == null)
                return CollectionStepNotFound();

            var response = await _aggregationService.AggregateAsync(step);
            return Ok(response);
        }

        [HttpPost("")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RequestAsync([FromRoute] Guid userId,
                                                      [FromBody] CollectionStepRequest request)
        {
            var initiation = _initiationProvider.Create(request.Reason);
            var newStepDto = _mapper.Map(request);
            var stepId = await _service.RequestAsync(userId, newStepDto, initiation);
            await request.TargetTasks.Foreach(taskId => _taskService.AddCollectionStepsAsync(taskId, new []{ stepId }, initiation));
            return NoContent();
        }

        [HttpPut("{stepId}/review")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReviewAsync([FromRoute] Guid userId,
                                                     [FromRoute] Guid stepId,
                                                     [FromBody] ReviewCollectionStepRequest request)
        {
            var step = await _stepStorage.FindAsync(stepId, userId);
            if (step == null)
                return CollectionStepNotFound();

            if (step.State != CollectionStepState.InReview)
                return CollectionStepReviewNotNeeded();

            var initiation = _initiationProvider.Create(request.Reason);
            await _service.ReviewAsync(stepId, request.ReviewResult, initiation);
            return NoContent();
        }

        [HttpDelete("{stepId}")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId,
                                                     [FromRoute] Guid stepId,
                                                     [FromBody] ReasonDto reasonDto)
        {
            var step = await _stepStorage.FindAsync(stepId, userId);
            if (step == null)
                return CollectionStepNotFound();
            if (step.State != CollectionStepState.Requested)
               return CollectionStepInvalidState(CollectionStepState.Requested);

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _service.RemoveAsync(stepId, initiation);
            return NoContent();
        }

        [HttpPatch("{stepId}")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid userId,
                                                     [FromRoute] Guid stepId,
                                                     [FromBody] UpdateCollectionStepRequest request)
        {
            var step = await _stepStorage.FindAsync(stepId, userId);
            if (step == null)
                return CollectionStepNotFound();
            if (step.State != CollectionStepState.Requested)
                return CollectionStepInvalidState(CollectionStepState.Requested);

            var initiation = _initiationProvider.Create(request.Reason);
            var collectionStepPatch = _mapper.Map(request);
            await _service.UpdateAsync(stepId, collectionStepPatch, initiation);
            return Ok();
        }
    }
}