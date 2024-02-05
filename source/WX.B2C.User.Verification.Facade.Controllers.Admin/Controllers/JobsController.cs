using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // Allow AccessGroup.Normal for job running in testing release
    [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel, AccessGroup.Normal)]
    [Route("api/v{version:apiVersion}/verification/jobs")]
    [DisableRequestSizeLimit]
    public class JobsController : ApiController
    {
        private readonly IJobService _jobService;
        private readonly IJobMapper _jobMapper;

        public JobsController(IJobService jobService, IJobMapper jobMapper)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _jobMapper = jobMapper ?? throw new ArgumentNullException(nameof(jobMapper));
        }

        [HttpPost("")]
        public async Task<IActionResult> ScheduleAsync([FromBody] ScheduleJobRequest request)
        {
            var requestDto = _jobMapper.Map(request);
            await _jobService.ScheduleAsync(requestDto);
            return Ok();
        }        
        
        [HttpDelete("")]
        public async Task<IActionResult> UnscheduleAsync([FromBody] UnscheduleJobRequest request)
        {
            var requestDto = _jobMapper.Map(request);
            await _jobService.UnscheduleAsync(requestDto);
            return Ok();
        }

        [HttpDelete("triggers/{fireInstanceId}")]
        public async Task<IActionResult> InterruptAsync([FromRoute] string fireInstanceId, CancellationToken cancellationToken)
        {
            await _jobService.InterruptAsync(fireInstanceId, cancellationToken);
            return Ok();
        }
    }
}
