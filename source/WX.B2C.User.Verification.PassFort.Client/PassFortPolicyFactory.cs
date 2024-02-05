using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace WX.B2C.User.Verification.PassFort.Client
{
    public interface IPassFortPolicyFactory
    {
        /// <summary>
        /// Create retry transient errors policy handler
        /// </summary>
        DelegatingHandler Create();
    }

    public class PassFortPolicyFactory : IPassFortPolicyFactory
    {
        /// <summary>
        /// https://help.passfort.com/article/nndr9v0sbf-about-rate-limiting
        /// </summary>
        private const string XRatelimitResetSecsHeader = "X-RateLimit-Reset-Secs";

        private readonly ILogger _logger;

        public PassFortPolicyFactory(ILogger logger)
        {
            _logger = logger?.ForContext<PassFortPolicyFactory>() ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create retry transient errors policy handler
        /// </summary>
        public DelegatingHandler Create() => new PolicyHttpMessageHandler(GetPolicy());

        private IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(10,
                                   SleepDurationProvider,
                                   onRetryAsync: (result, timeSpan, attempt, context) =>
                                   {
                                       _logger.Warning(result.Exception,
                                                       "Begin {RetryNumber} retry on failed request to PassFort with error message: {ErrorMessage}",
                                                       attempt,
                                                       result.Exception?.Message ?? result.Result?.ReasonPhrase);
                                       return Task.CompletedTask;
                                   });

        private TimeSpan SleepDurationProvider(int retryAttempt, DelegateResult<HttpResponseMessage> message, Context context)
        {
            var headers = message.Result?.Headers;
            if (headers != null && headers.TryGetValues(XRatelimitResetSecsHeader, out var recommendedValues))
            {
                var recommendedRetryInSeconds = recommendedValues.FirstOrDefault();
                if (recommendedRetryInSeconds != null)
                    return TimeSpan.FromSeconds(double.Parse(recommendedRetryInSeconds) + retryAttempt);
            }

            _logger.Warning("PassFort didn't provide any recommended value for retry. Will be used exponential backoff.");
            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        }
    }
}
