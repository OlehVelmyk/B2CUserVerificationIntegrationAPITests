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
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
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

            return MapException(exception, errorResponse);
        }

        private IActionResult MapException(Exception exception, ErrorResponse errorResponse) =>
            exception switch
            {
                BlobStorageFileNotFoundException _             => NotFound(errorResponse),
                InvalidStateTransitionException _              => Conflict(errorResponse),
                InconsistentRevertDecisionOperationException _ => Conflict(errorResponse),
                ApproveApplicationException _                  => Conflict(errorResponse),
                ApplicationTaskAlreadyExistsException _        => Conflict(errorResponse),
                CheckAlreadyCompletedException _               => Conflict(errorResponse),
                CollectionStepReviewRequiredException _        => Conflict(errorResponse),
                TaskAlreadyCompletedException _                => Conflict(errorResponse),
                InvalidStateException _                        => Conflict(errorResponse),
                TaskChecksNotCompletedException _              => BadRequest(errorResponse),
                TaskExpiredException                           => BadRequest(errorResponse),
                TaskStepsNotCompletedException                 => BadRequest(errorResponse),
                UserMismatchedException                        => BadRequest(errorResponse),

                _ => StatusCode(StatusCodes.Status500InternalServerError, errorResponse)
            };

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
                B2CVerificationException _ => BusinessError(exception.Message),
                _ => ApplicationError(Constants.ErrorMessages.ApplicationError)
            };

            static ErrorDetails BusinessError(string message) => ErrorDetails.Create(Constants.ErrorCodes.BusinessError, message);
            static ErrorDetails ApplicationError(string message) => ErrorDetails.Create(Constants.ErrorCodes.ApplicationError, message);
        }
    }
}