using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Microsoft.Rest;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.LexisNexis
{
    internal interface IRdpApiClientFactory
    {
        Settings GetSettings();

        ILexisNexisRdpApiClient Create();
    }

    public class RdpApiClientSettings
    {
        public Uri Host { get; set; }

        public Uri ProxyHost { get; set; }

        public string ApiKeyId { get; set; }

        public string ApiSecretKey { get; set; }

        public string AccountId { get; set; }

        public string WorkflowName { get; set; }

        public string Mode { get; set; }
    }

    internal class RdpApiClientFactory : IRdpApiClientFactory
    {
        private readonly RdpApiClientSettings _settings;
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly ILogger _logger;

        public RdpApiClientFactory(
            RdpApiClientSettings settings,
            IOperationContextProvider operationContextProvider,
            ILogger logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Settings GetSettings()
        {
            var operationContext = _operationContextProvider.GetContextOrDefault();
            return new Settings
            {
                Reference = operationContext.CorrelationId.ToString(),
                Mode = _settings.Mode,
                Locale = "en_US",
                Venue = "online"
            };
        }

        public ILexisNexisRdpApiClient Create()
        {
            var credentials = new RdpApiClientCredentials(_settings.ApiKeyId, _settings.ApiSecretKey);
            var proxyHandler = new HttpClientHandler { Proxy = new WebProxy(_settings.ProxyHost) };
            var retryHandler = new PolicyHttpMessageHandler(PolicyBuilder.RetryTransientErrorsPolicy(_logger));
            return new LexisNexisRdpApiClient(credentials, proxyHandler, retryHandler)
            {
                BaseUri = _settings.Host,
                AccountId = _settings.AccountId,
                WorkflowName = _settings.WorkflowName
            };
        }

        private class RdpApiClientCredentials : ServiceClientCredentials
        {
            private const string AuthorizationHeaderPrefix = "HMAC-SHA256 "; // we need the space in the end of this prefix
            private const string KeyIdLabel = "keyid";
            private const string NonceLabel = "nonce";
            private const string TsLabel = "ts";
            private const string BodyHashLabel = "bodyhash";
            private const string SignatureLabel = "signature";

            private readonly string _apiKeyId;
            private readonly string _apiSecretKey;

            public RdpApiClientCredentials(string apiKeyId, string apiSecretKey)
            {
                _apiKeyId = apiKeyId ?? throw new ArgumentNullException(nameof(apiKeyId));
                _apiSecretKey = apiSecretKey ?? throw new ArgumentNullException(nameof(apiSecretKey));
            }

            public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var requestContent = await request.Content.ReadAsStringAsync();
                var authorization = GetAuthorizationHeader(request.RequestUri, requestContent);
                request.Headers.TryAddWithoutValidation("Authorization", authorization);

                await base.ProcessHttpRequestAsync(request, cancellationToken);
            }

            private string GetAuthorizationHeader(Uri requestUri, string body)
            {
                // ts using time in milliseconds
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                // nonce must be unique for each request
                var nonce = Guid.NewGuid().ToString();

                // create the bodyHash value by hashing the payload and encoding it
                var bodyHash = GetHash(body);

                // this string will be used to generate mac signature.
                var sb = new StringBuilder(400);
                const string newline = "\n";
                sb.Append(ts).Append(newline).Append(nonce).Append(newline)
                  .Append(requestUri.Host).Append(newline).Append(requestUri.PathAndQuery).Append(newline)
                  .Append(bodyHash);

                // Generate signature using client secret (crypto initialized above)
                var signatureStr = GetHash(sb.ToString());

                var headerStringBuilder = new StringBuilder(400);
                return headerStringBuilder
                       .Append(AuthorizationHeaderPrefix)
                       .Append(KeyIdLabel).Append("=").Append(_apiKeyId)
                       .Append(",").Append(TsLabel).Append("=").Append(ts)
                       .Append(",").Append(NonceLabel).Append("=").Append(nonce)
                       .Append(",").Append(BodyHashLabel).Append("=").Append(bodyHash)
                       .Append(",").Append(SignatureLabel).Append("=").Append(signatureStr)
                       .ToString();
            }

            private string GetHash(string value)
            {
                var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecretKey));
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
