using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Services;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger?.ForContext<ErrorController>() ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;

            _logger.Error(exception, "Request execution failed.");

            return exception switch
            {
                SecretValidationException => StatusCode(StatusCodes.Status403Forbidden),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
