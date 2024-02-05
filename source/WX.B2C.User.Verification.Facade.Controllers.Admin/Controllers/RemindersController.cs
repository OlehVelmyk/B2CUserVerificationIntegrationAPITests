using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
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
    [Route("api/v{version:apiVersion}/verification/{userId}/reminders/")]
    public class RemindersController : ApiController
    {
        private readonly IReminderStorage _reminderStorage;
        private readonly IJobService _jobService;
        private readonly IReminderMapper _reminderMapper;

        public RemindersController(IReminderStorage reminderStorage, IJobService jobService, IReminderMapper reminderMapper)
        {
            _reminderStorage = reminderStorage ?? throw new ArgumentNullException(nameof(reminderStorage));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _reminderMapper = reminderMapper ?? throw new ArgumentNullException(nameof(reminderMapper));
        }
        
        [HttpGet("active")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ActiveReminderDto[]>> GetActiveRemindersAsync([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var jobParameters = _reminderMapper.MapToJob(userId);
            var triggers = await _jobService.GetJobTriggersAsync(jobParameters, cancellationToken);
            return Ok(triggers.Select(_reminderMapper.Map));
        }

        [HttpGet("sent")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReminderDto[]>> GetSentRemindersAsync([FromRoute] Guid userId)
        {
            var reminders = await _reminderStorage.FindAsync(userId);
            return Ok(reminders.Select(_reminderMapper.Map));
        }
    }
}
