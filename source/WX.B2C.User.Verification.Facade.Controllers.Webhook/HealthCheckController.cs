using System;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook
{
    [ApiController]
    [Route("healthcheck")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ISystemClock _systemClock;

        public HealthCheckController(ISystemClock systemClock)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        [HttpPost("ping")]
        public IActionResult Post()
        {
            return Ok(_systemClock.GetDate());
        }
    }
}