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
    [Route("api/v{version:apiVersion}/verification/{userId}/tasks")]
    public class TasksController : ApiController
    {
        private readonly ITaskStorage _taskStorage;
        private readonly ITaskService _taskService;
        private readonly ITaskAggregationService _aggregationService;
        private readonly IInitiationProvider _initiationProvider;

        public TasksController(
            ITaskStorage taskStorage,
            ITaskService taskService,
            ITaskAggregationService aggregationService,
            IInitiationProvider initiationProvider)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TaskDto[]>> GetAllAsync([FromRoute] Guid userId)
        {
            var tasks = await _taskStorage.GetAllAsync(userId);
            var response = await _aggregationService.AggregateAsync(tasks);
            return Ok(response);
        }

        [HttpGet("{taskId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TaskDto>> GetAsync([FromRoute] Guid userId,
                                                          [FromRoute] Guid taskId)
        {
            var task = await _taskStorage.FindAsync(taskId, userId);
            if (task == null)
                return TaskNotFound();
            
            var response = await _aggregationService.AggregateAsync(task);
            return Ok(response);
        }

        [HttpPost("{taskId}/complete")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CompleteAsync([FromRoute] Guid userId,
                                                       [FromRoute] Guid taskId,
                                                       [FromBody] CompleteTaskRequest request)
        {
            var task = await _taskStorage.FindAsync(taskId, userId);
            if (task == null)
                return TaskNotFound();

            var initiation = _initiationProvider.Create(request.Reason);
            await _taskService.CompleteAsync(taskId, request.Result, initiation);
            return NoContent();
        }

        [HttpPost("{taskId}/incomplete")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> IncompleteAsync([FromRoute] Guid userId,
                                                         [FromRoute] Guid taskId,
                                                         [FromBody] ReasonDto reasonDto)
        {
            var task = await _taskStorage.FindAsync(taskId, userId);
            if (task == null)
                return TaskNotFound();

            var initiation = _initiationProvider.Create(reasonDto.Reason);
            await _taskService.IncompleteAsync(taskId, initiation);
            return NoContent();
        }

        [HttpDelete("{taskId}/collection-steps/{collectionStepId}")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveCollectionStepAsync([FromRoute] Guid userId,
                                                                   [FromRoute] Guid taskId,
                                                                   [FromRoute] Guid collectionStepId)
        {
            var task = await _taskStorage.FindAsync(taskId, userId);
            if (task == null)
                return TaskNotFound();

            await _taskService.RemoveCollectionStepAsync(taskId, collectionStepId);
            return NoContent();
        }
    }
}