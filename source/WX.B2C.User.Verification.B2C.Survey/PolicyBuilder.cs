using System;
using System.Net;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace WX.B2C.User.Verification.B2C.Survey
{
    internal class PolicyBuilder
    {
        internal static IAsyncPolicy<HttpResponseMessage> RetryTransientErrorsPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                   .HandleTransientHttpError()
                   .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
                   .WaitAndRetryAsync(
                       retryCount: 5,
                       sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                       onRetry: (result, timeSpan, retries, context) =>
                           logger.Warning(
                               result.Exception,
                               "Begin {RetryNumber} retry on failed request to B2C.Survey with error message: {ErrorMessage}",
                               retries,
                               result.Exception?.Message ?? result.Result?.ReasonPhrase));
        }
    }
}