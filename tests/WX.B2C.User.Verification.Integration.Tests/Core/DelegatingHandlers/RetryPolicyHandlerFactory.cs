using System.Net;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;

/// <remarks>
/// Retry on BadRequest or Conflict will be removed after all check will be implemented in tests
/// </remarks>
internal class RetryPolicyHandlerFactory
{
    private readonly ILogger _logger;

    public RetryPolicyHandlerFactory(ILogger logger)
    {
        _logger = logger;
    }
    public DelegatingHandler Create() => new PolicyHttpMessageHandler(GetPolicy());

    private IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
        HttpPolicyExtensions.HandleTransientHttpError()
                            .OrResult(message => message.StatusCode is HttpStatusCode.TooManyRequests 
                                                                    or HttpStatusCode.BadRequest 
                                                                    or HttpStatusCode.Conflict
                                                                    or HttpStatusCode.NotFound)
                            .WaitAndRetryAsync(
                                retryCount: Timeouts.DefaultRetryAttempts, 
                                sleepDurationProvider: Timeouts.GetTimeout,
                                (result, timeout, retryAttempt, _) => 
                                    _logger.Warning(GetLogString(result, timeout, retryAttempt)));

    private static string GetLogString(DelegateResult<HttpResponseMessage> result, TimeSpan timeout, int retryAttempt) =>
        $"Failed request to {result.Result?.RequestMessage?.RequestUri} " +
        $"with exception {result.Exception} " +
        $"with response status code {result.Result?.StatusCode} " +
        $"with response : {result.Result?.Content.ReadAsStringAsync().Result} " +
        $"retry attempt {retryAttempt}, timeout {timeout}";
}
