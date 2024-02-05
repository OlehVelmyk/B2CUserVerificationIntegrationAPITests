using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Filters
{
    public class RequestLoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;

        public RequestLoggingFilter(ILogger logger)
        {
            _logger = logger?.ForContext<RequestLoggingFilter>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = _logger.ForContext("ActionName", context.ActionDescriptor.DisplayName)
                                .ForContext("RequestParameters", context.ActionArguments, true);
            
            logger.Information("Starting to execute action");

            var started = DateTime.UtcNow;
            try
            {
                var result = await next();
                
                if (!IsSuccessful(result))
                    logger.Warning("Unsuccessful response: {@Response}", result.Result);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error during request processing");
            }
            finally
            {
                var elapsedMilliseconds = (DateTime.UtcNow - started).TotalMilliseconds;
                logger.Information("Finished executing action  Took: {Elapsed} ms",
                                   elapsedMilliseconds);
            }
        }

        ///  <summary>
        /// https://docs.microsoft.com/en-us/uwp/api/windows.web.http.httpresponsemessage.issuccessstatuscode?view=winrt-22000#property-value 
        ///  </summary>
        private bool IsSuccessful(ActionExecutedContext context)
        {
            if (context.Result is not IStatusCodeActionResult statusCodeActionResult)
                return true;

            if (statusCodeActionResult.StatusCode.HasValue)
                return statusCodeActionResult.StatusCode is >= 200 and <= 299;

            _logger.Error("Status code is not defined. Probably overload Ok or NoContent missed in {ActionDescription}",
                          context.ActionDescriptor.DisplayName);
            return true;
        }
    }
}
