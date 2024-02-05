using System;
using Microsoft.Rest;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public interface IOnfidoApiClientFactory
    {
        IOnfidoApiClient Create(string region = "eu");
    }

    public class OnfidoApiClientFactory : IOnfidoApiClientFactory
    {
        private readonly IOnfidoPolicyFactory _onfidoPolicyFactory;
        private readonly OnfidoClientSettings _onfidoClientSettings;

        public OnfidoApiClientFactory(IOnfidoPolicyFactory onfidoPolicyFactory, OnfidoClientSettings onfidoClientSettings)
        {
            _onfidoPolicyFactory = onfidoPolicyFactory ?? throw new ArgumentNullException(nameof(onfidoPolicyFactory));
            _onfidoClientSettings = onfidoClientSettings ?? throw new ArgumentNullException(nameof(onfidoClientSettings));
        }

        public IOnfidoApiClient Create(string region = "eu")
        {
            var credentials = new TokenCredentials(_onfidoClientSettings.Token, _onfidoClientSettings.TokenType);
            var retryPolicy = _onfidoPolicyFactory.Create();
            var client = retryPolicy == null
                ? new OnfidoApiClient(credentials)
                : new OnfidoApiClient(credentials, retryPolicy);

            client.BaseUri = _onfidoClientSettings.OnfidoApiUrl;
            client.Region = region;
            client.HttpClient.Timeout = TimeSpan.FromMinutes(10);

            return client;
        }
    }
}
