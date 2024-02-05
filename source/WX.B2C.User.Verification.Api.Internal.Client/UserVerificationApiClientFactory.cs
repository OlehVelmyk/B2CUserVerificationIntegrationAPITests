using System;

namespace WX.B2C.User.Verification.Api.Internal.Client
{
    public interface IUserVerificationApiClientFactory
    {
        IUserVerificationApiClient Create(Guid correlationId);
    }

    public class UserVerificationApiClientFactory : IUserVerificationApiClientFactory
    {
        private readonly UserVerificationClientSettings _settings;

        public UserVerificationApiClientFactory(UserVerificationClientSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IUserVerificationApiClient Create(Guid correlationId)
        {
            return new UserVerificationApiClient(_settings.BaseUri) { CorrelationId = correlationId };
        }
    }
}
