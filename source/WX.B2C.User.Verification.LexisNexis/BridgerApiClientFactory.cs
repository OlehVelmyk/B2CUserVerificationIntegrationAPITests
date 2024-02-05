using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.LexisNexis.Bridger.Client;

namespace WX.B2C.User.Verification.LexisNexis
{
    public class BridgerApiClientSettings
    {
        public Uri BaseUri { get; set; }

        public string ClientId { get; set; }

        public string RolesOrUsers { get; set; }

        public string UserId { get; set; }
    }

    public interface IBridgerApiClientFactory
    {
        Task<BridgerApiClient> CreateAsync();
    }

    public class BridgerApiClientFactory : IBridgerApiClientFactory
    {
        private readonly BridgerApiClientSettings _settings;
        private readonly IBridgerCredentialsProvider _bridgerCredentialsProvider;
        private readonly IOperationContextProvider _operationContextProvider;

        public BridgerApiClientFactory(
            BridgerApiClientSettings settings,
            IBridgerCredentialsProvider bridgerCredentialsProvider, 
            IOperationContextProvider operationContextProvider)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _bridgerCredentialsProvider = bridgerCredentialsProvider ?? throw new ArgumentNullException(nameof(bridgerCredentialsProvider));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
        }

        public async Task<BridgerApiClient> CreateAsync()
        {
            var credentials = await GetAsync();
            var context = _operationContextProvider.GetContextOrDefault();

            return new BridgerApiClient(_settings.BaseUri, credentials)
            {
                Reference = context.CorrelationId.ToString(),
                RolesOrUsers = _settings.RolesOrUsers
            };
        }

        private async Task<BridgerCredentials> GetAsync()
        {
            var password = await _bridgerCredentialsProvider.GetPasswordAsync(_settings.UserId);

            return new BridgerCredentials
            {
                ClientId = _settings.ClientId,
                UserId = _settings.UserId,
                Password = password
            };
        }
    }
}
