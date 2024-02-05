using System;
using System.Collections.Generic;
using System.Security;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;

namespace WX.B2C.User.Verification.Infrastructure.Common.Configuration
{
    internal class AppConfig : IAppConfig
    {
        private readonly IB2CUserVerificationKeyVault _keyVault;
        private readonly IMessagingKeyVault _messagingKeyVault;
        private readonly IConfigurationKeyVault _configurationKeyVault;
        private readonly IHostSettingsProvider _hostSettingsProvider;

        public AppConfig(
            IB2CUserVerificationKeyVault keyVault,
            IMessagingKeyVault messagingKeyVault,
            IConfigurationKeyVault configurationKeyVault,
            IHostSettingsProvider hostSettingsProvider)
        {
            _keyVault = keyVault ?? throw new ArgumentNullException(nameof(keyVault));
            _messagingKeyVault = messagingKeyVault ?? throw new ArgumentNullException(nameof(messagingKeyVault));
            _configurationKeyVault = configurationKeyVault ?? throw new ArgumentNullException(nameof(configurationKeyVault));
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
        }

        public SecureString SplunkEndpoint => _keyVault.SplunkEndpoint;

        public SecureString SplunkToken => _keyVault.SplunkToken;

        public SecureString DbConnectionString => _keyVault.DbConnectionString;

        public Dictionary<string, SecureString> EventHubNameSpaceConnectionStrings => _messagingKeyVault.EventHubNameSpaceConnectionStrings;

        public SecureString StorageConnectionString => _messagingKeyVault.StorageConnectionString;

        public SecureString B2CStorageConnectionString => _messagingKeyVault.B2CStorageConnectionString;

        public string CommandsQueueName => CommandQueueNameResolver.Get();

        public SecureString CommandsQueueConnectionString => _messagingKeyVault.B2CStorageConnectionString;

        public SecureString EventHubPrivateKey => _messagingKeyVault.EventHubPrivateKey;

        public SecureString EventHubPublicKey => _messagingKeyVault.EventHubPublicKey;

        public SecureString AppInsightsInstrumentationKey => _keyVault.AppInsightsInstrumentationKey;

        public SecureString ApplicationConfigurationConnectionString => _configurationKeyVault.ApplicationConfigurationConnectionString;

        public string ServiceName => "b2c-verification";

        public SecureString IpStackAccessKey => _keyVault.IpStackToken;

        public string IpStackApiUrl => _hostSettingsProvider.GetSetting(nameof(IpStackApiUrl));

        public SecureString OnfidoApiToken => _keyVault.OnfidoApiToken;

        public string OnfidoApiUrl => _hostSettingsProvider.GetSetting(nameof(OnfidoApiUrl));

        public string LexisNexisRdpHost => _hostSettingsProvider.GetSetting(nameof(LexisNexisRdpHost));

        public string LexisNexisRdpProxy => _hostSettingsProvider.GetSetting(nameof(LexisNexisRdpProxy));

        public string LexisNexisMode => _hostSettingsProvider.GetSetting(nameof(LexisNexisMode));

        public string LexisNexisBridgerServiceEndpoint => _hostSettingsProvider.GetSetting(nameof(LexisNexisBridgerServiceEndpoint));

        public string PassFortApiUrl => _hostSettingsProvider.GetSetting(nameof(PassFortApiUrl));

        public string B2CRisksServiceUrl => "fabric:/WX.B2C.Risks/ApiService";

        public string B2CSurveyServiceUrl => "fabric:/WX.B2C.Survey/InternalApi";

        public string SurveyServiceUrl => "fabric:/WX.Survey/WX.Survey.Api";

        public string B2CRisksApiUrl => _hostSettingsProvider.GetSetting(nameof(B2CRisksApiUrl));

        public string B2CSurveyApiUrl => _hostSettingsProvider.GetSetting(nameof(B2CSurveyApiUrl));

        public string SurveyApiUrl => _hostSettingsProvider.GetSetting(nameof(SurveyApiUrl));

        public bool CheckAutofacRegistrations => false;

        public bool IsLocal => false;
    }
}
