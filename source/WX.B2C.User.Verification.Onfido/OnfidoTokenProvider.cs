using System;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido
{
    internal class OnfidoTokenProvider : BaseOnfidoGateway, IOnfidoTokenProvider
    {
        private const string OnfidoReferrer = nameof(OnfidoReferrer);

        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly IHostSettingsProvider _hostSettingsProvider;

        public OnfidoTokenProvider(
            IOnfidoApiClientFactory clientFactory,
            IHostSettingsProvider hostSettingsProvider,
            ILogger logger)
            : base(logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
        }

        public Task<string> CreateWebTokenAsync(string applicantId)
        {
            if (string.IsNullOrEmpty(applicantId))
                throw new ArgumentNullException(nameof(applicantId));

            var request = new SdkTokenRequest
            {
                ApplicantId = applicantId,
                Referrer = _hostSettingsProvider.GetSetting(OnfidoReferrer)
            };

            return CreateTokenAsync(request);
        }

        public Task<string> CreateMobileTokenAsync(string applicantId, string applicationId)
        {
            if (string.IsNullOrEmpty(applicantId))
                throw new ArgumentNullException(nameof(applicantId));

            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException(nameof(applicationId));

            var request = new SdkTokenRequest
            {
                ApplicantId = applicantId,
                ApplicationId = applicationId
            };

            return CreateTokenAsync(request);
        }

        private async Task<string> CreateTokenAsync(SdkTokenRequest request)
        {
            using var client = _clientFactory.Create();
            return await HandleAsync(
                requestFactory: () => request,
                requestInvoker: client.SdkToken.GenerateAsync,
                responseMapper: token => token.Token);
        }
    }
}
