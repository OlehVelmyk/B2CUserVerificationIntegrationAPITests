using System;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Fixtures;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    internal class PublicApiClientFactory
    {
        private readonly Uri _endpoint;

        public PublicApiClientFactory(string endpoint)
        {
            _endpoint = endpoint != null ? new Uri(endpoint) : throw new ArgumentNullException(nameof(endpoint));
        }

        public IUserVerificationApiClient Create(Guid userId, string ipAddress = "0.0.0.0", Guid? correlationId = null, Guid? operationId = null)
        {
            var credentials = TokenFixture.GenerateCredentials(userId, ipAddress);
            return new UserVerificationApiClient(_endpoint, credentials)
            {
                CorrelationId = correlationId ?? Guid.NewGuid(),
                OperationId = operationId ?? Guid.NewGuid()
            };
        }
    }
}
