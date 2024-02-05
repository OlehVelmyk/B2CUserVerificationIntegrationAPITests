using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger?.ForContext<ErrorController>();
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;

            _logger.Error(exception, "Request execution failed.");

            var errorDetails = GetErrorDetails(exception);
            var errorResponse = ErrorResponse.Create(errorDetails.ToArray());

            return exception switch
            {
                _ => StatusCode(StatusCodes.Status500InternalServerError, errorResponse)
            };
        }

        private static IEnumerable<ErrorDetails> GetErrorDetails(Exception exception)
        {
            var exceptions = Enumerable.Repeat(exception, 1);

            if (exception is AggregateException aggregateException)
            {
                exceptions = aggregateException
                             .InnerExceptionsOfType<B2CVerificationException>()
                             .DefaultIfEmpty(exception);
            }

            return exceptions.Select(MapException);
        }

        private static ErrorDetails MapException(Exception exception)
        {
            return exception switch
            {
                B2CVerificationException _ => ErrorDetails.Create(Constants.ErrorCodes.BusinessError, exception.Message),
                _ => ErrorDetails.Create(Constants.ErrorCodes.ApplicationError, Constants.ErrorMessages.ApplicationError)
            };
        }
    }
}