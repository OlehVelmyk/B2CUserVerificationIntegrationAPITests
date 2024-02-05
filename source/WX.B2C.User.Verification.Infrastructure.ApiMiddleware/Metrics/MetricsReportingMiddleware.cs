using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Metrics
{
    public class MetricsReportingMiddleware : IMiddleware
    {
        private readonly IMetricsLogger _metricsLogger;

        public MetricsReportingMiddleware(IMetricsLogger metricsLogger)
        {
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next) =>
            _metricsLogger.ExecuteWithHttpRequestTracking(() => next.Invoke(context), 
                                                          () => context.Response.StatusCode, 
                                                          () => GetError(context));

        private static Exception GetError(HttpContext context)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;
            
            if (exception is AggregateException ae)
                exception = ae.GetBaseException();

            return exception;
        }
    }
}