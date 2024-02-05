using System;
using System.Net;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Microsoft.Extensions.Http;
using WX.B2C.User.Verification.Onfido.Client;

namespace WX.B2C.User.Verification.Onfido
{
    internal class OnfidoPolicyFactory : IOnfidoPolicyFactory
    {
        private const string OngoingChecksType = "incomplete_checks";
        private readonly ILogger _logger;
        private Random _jitterer = new ();

        public OnfidoPolicyFactory(ILogger logger)
        {
            _logger = logger?.ForContext<OnfidoPolicyFactory>() ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create retry transient errors policy handler
        /// </summary>
        public DelegatingHandler Create()
        {
            var policy = Policy.WrapAsync(TransientErrorPolicy(),
                                          OngoingCheckPolicy(),
                                          NotFoundErrorPolicy());
            
            return new PolicyHttpMessageHandler(policy);
        }

        private IAsyncPolicy<HttpResponseMessage> TransientErrorPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (result, timeSpan, retries, context) =>
                        _logger.Warning(
                            result.Exception,
                            "Begin {RetryNumber} retry on failed request to Onfido with error message: {ErrorMessage}",
                            retries,
                            result.Exception?.Message ?? result.Result?.ReasonPhrase));

        /// <summary>
        /// Sometimes onfido returns "not found" for resources that actually exist.
        /// Issue is on Onfido side. Retry policy can help handle such situations properly.  
        /// </summary>
        private IAsyncPolicy<HttpResponseMessage> NotFoundErrorPolicy() =>
            Policy<HttpResponseMessage>
                .HandleResult(message => message.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                retryCount: 4,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                onRetry: (result, _, retries, _) =>
                    _logger.Warning(
                    result.Exception,
                    "Begin {RetryNumber} retry on resource not found response from Onfido with error message: {ErrorMessage}",
                    retries,
                    result.Exception?.Message ?? result.Result?.ReasonPhrase));
        
        private IAsyncPolicy<HttpResponseMessage> OngoingCheckPolicy() =>
            Policy<HttpResponseMessage>
                .HandleResult(message => message.StatusCode == HttpStatusCode.UnprocessableEntity && HasOtherOngoingCheck(message.Content))
                .WaitAndRetryAsync(retryCount: 10,
                                   sleepDurationProvider: _ => TimeSpan.FromSeconds(60) + TimeSpan.FromMilliseconds(_jitterer.Next(0, 2000)),
                                   onRetry: (result,
                                             _,
                                             retries,
                                             _) =>
                                       _logger.Warning(result.Exception,
                                                       "Begin {RetryNumber} retry on failed request to Onfido with error message: {ErrorMessage}",
                                                       retries,
                                                       "Another check in progress"));
        private static bool HasOtherOngoingCheck(HttpContent messageContent)
        {
            var response = messageContent.ReadAsStringAsync().GetAwaiter().GetResult();
            var hasOtherOngoingCheck = response.Contains(OngoingChecksType);
            return hasOtherOngoingCheck;
        }
    }
}
