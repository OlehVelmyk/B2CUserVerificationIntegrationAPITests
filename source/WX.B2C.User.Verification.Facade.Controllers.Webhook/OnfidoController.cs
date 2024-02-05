using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Helpers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook
{
    [ApiController]
    [Route("onfido")]
    public class OnfidoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public OnfidoController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger?.ForContext<OnfidoController>() ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("events")]
        public async Task<IActionResult> PostAsync([FromBody] OnfidoEventRequest request)
        {
            if (request == null)
                return BadRequest();

            var @object = request.Payload.Object;
            var resourceType = request.Payload.ResourceType;
            var handleCommand = resourceType switch
            {
                ResourceTypes.Check => HandleCheckAsync(@object),
                ResourceTypes.Report => HandleReportAsync(@object),
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, "Unknown Onfido webhook type.")
            };

            await handleCommand;

            return Ok();
        }

        private Task HandleCheckAsync(Requests.Object @object)
        { 
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (@object.Status != CheckStatus.Complete)
            {
                _logger.Information("Check is not completed at the moment.");
                return Task.CompletedTask;
            }

            var applicantId = OnfidoHelper.GetApplicantIdFromLink(@object.Href);
            var command = CompleteOnfidoCheck.Create(applicantId, @object.Id);
            return _mediator.Send(command);
        }

        private Task HandleReportAsync(Requests.Object @object)
        {
            _logger.ForContext("Report", @object)
                   .Information("Report events currently are not supported, therefore skipped.");

            return Task.CompletedTask;
        }
    }
}
