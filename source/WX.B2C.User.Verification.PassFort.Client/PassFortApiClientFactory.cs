using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace WX.B2C.User.Verification.PassFort.Client
{
    public interface IPassFortApiClientFactory
    {
        IPassFortApiClient Create();
    }

    public class PassFortApiClientFactory : IPassFortApiClientFactory
    {
        private readonly PassFortApiSettings _settings;
        private readonly IPassFortPolicyFactory _passFortPolicyFactory;

        public PassFortApiClientFactory(PassFortApiSettings settings, IPassFortPolicyFactory passFortPolicyFactory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _passFortPolicyFactory = passFortPolicyFactory ?? throw new ArgumentNullException(nameof(passFortPolicyFactory));
        }

        public IPassFortApiClient Create()
        {
            var credentials = new PassFortApiClientCredentials(_settings.ApiKey);
            var retryPolicy = _passFortPolicyFactory.Create();
            var passFortApiClient = new PassFortApiClient(_settings.ApiUri, credentials, retryPolicy);
            passFortApiClient.HttpClient.Timeout = TimeSpan.FromMinutes(10);
            return passFortApiClient;
        }

        private class PassFortApiClientCredentials : ServiceClientCredentials
        {
            private const string AuthHeader = "APIKEY";
            private readonly string _apiSecret;

            public PassFortApiClientCredentials(string apiSecret)
            {
                _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                request.Headers.Add(AuthHeader, _apiSecret);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
    }
}
