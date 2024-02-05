using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using ErrorCodes = WX.B2C.User.Verification.Facade.Controllers.Public.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
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

            // TODO: Open question
            // When we get AggregateException it returns status code 500
            // Partially it is fixed, also need to resolve issue that sounds like
            // "What to do if AggregateException is consisted of more than 1 exception" 
            if (exception is AggregateException { InnerException: { } } ae)
                exception = ae.InnerException;

            var errorDetails = GetErrorDetails(exception);
            var errorResponse = ErrorResponse.Create(errorDetails.ToArray());            

            return exception switch
            {
                ExternalFileNotFoundException _    => NotFound(errorResponse),
                _                                  => StatusCode(StatusCodes.Status500InternalServerError, errorResponse)
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
                ExternalFileNotFoundException _  => ErrorDetails.Create(ErrorCodes.ExternalFileNotFound, exception.Message),
                B2CVerificationException _       => ErrorDetails.Create(ErrorCodes.BusinessError, exception.Message),
                _                                => ErrorDetails.Create(ErrorCodes.ApplicationError, Constants.ErrorMessages.ApplicationError)
            };
        }
    }
}
