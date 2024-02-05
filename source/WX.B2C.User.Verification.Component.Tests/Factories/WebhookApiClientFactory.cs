using System;
using WX.B2C.User.Verification.Api.Webhook.Client;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal class WebhookApiClientFactory
    {
        private readonly Uri _endpoint;

        public WebhookApiClientFactory(string endpoint)
        {
            _endpoint = new Uri(endpoint);
        }

        public IWebhookApiClient Create() => new WebhookApiClient(_endpoint);
    }
}