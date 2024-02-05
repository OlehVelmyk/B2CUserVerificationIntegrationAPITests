using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook
{
    [ApiController]
    [Route("passfort")]
    public class PassFortController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPassFortWebhookMapper _mapper;
        private readonly ILogger _logger;
        
        public PassFortController(IMediator mediator, IPassFortWebhookMapper mapper, ILogger logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger?.ForContext<PassFortController>() ?? throw new ArgumentNullException(nameof(logger));
        }

        [WebhookSecretFilter]
        [HttpPost("events")]
        public async Task<IActionResult> PostAsync([FromBody] PassFortEventRequest request)
        {
            if (request == null)
                return BadRequest();

            if (request is UnsupportedPassFortEventRequest)
            {
                _logger
                    .ForContext(nameof(request), request, true)
                    .Warning("Skipped unsupported PassFort event {EventType}.", request.Event);
                return Ok();
            }

            var command = _mapper.Map(request);
            await _mediator.Send(command);

            return Ok();
        }
    }
}
