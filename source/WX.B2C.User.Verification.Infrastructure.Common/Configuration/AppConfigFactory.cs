using System;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;

namespace WX.B2C.User.Verification.Infrastructure.Common.Configuration
{
    public static class AppConfigFactory
    {
        public static IAppConfig Create(Func<string> environmentFactory,
                                        Func<IB2CUserVerificationKeyVault> keyVaultFactory,
                                        Func<IMessagingKeyVault> messagingKeyVaultFactory,
                                        Func<IConfigurationKeyVault> configurationKeyVaultFactory,
                                        Func<IHostSettingsProvider> hostSettingsProviderFactory)
        {
            var env = environmentFactory().ToLowerInvariant();
            return env switch
            {
                "local" => new AppLocalConfig(),
                _ => new AppConfig(keyVaultFactory(), messagingKeyVaultFactory(), configurationKeyVaultFactory(), hostSettingsProviderFactory())
            };
        }
    }
}
