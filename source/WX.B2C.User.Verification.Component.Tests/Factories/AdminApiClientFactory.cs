using System;
using System.Net.Http;
using Microsoft.Rest;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal class AdminApiClientFactory
    {
        private readonly Uri _endpoint;

        public AdminApiClientFactory(string endpoint)
        {
            _endpoint = new Uri(endpoint);
        }

        public IUserVerificationApiClient Create(Administrator admin, Guid? correlationId = null, Guid? operationId = null)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
            return new UserVerificationApiClient(_endpoint, new TokenCredentials(admin.AccessToken, admin.TokenType), handler)
            {
                CorrelationId = correlationId ?? Guid.NewGuid(),
                OperationId = operationId ?? Guid.NewGuid()
            };
        }
    }
}