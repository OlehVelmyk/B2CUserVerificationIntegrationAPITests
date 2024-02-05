using System;
using Autofac;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Configuration.Contracts;
using WX.KeyVault;

namespace WX.B2C.User.Verification.Integration.Tests.Configuration
{
    public static class ConfigurationFixture
    {
        internal static void RegisterEnvProxySettings(this ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(_ => new SelfHostingValuesResolver("AppConfig", "KeyVault"))
                            .As<IHostingSpecificValuesResolver>();

            containerBuilder.Register(c =>
                            {
                                var settingsProvider = c.Resolve<IHostingSpecificValuesResolver>();
                                var url = settingsProvider.GetValue("KeyVaultUrl");
                                var clientId = settingsProvider.GetValue("KeyVaultClientId");
                                var secret = settingsProvider.GetValue("KeyVaultSecret");
                                return new KeyVaultConfiguration(url, clientId, secret);
                            })
                            .As<IKeyVaultConfiguration>();

            RegisterKeyVault<IMessagingKeyVault>(containerBuilder);
            RegisterKeyVault<IConfigurationKeyVault>(containerBuilder);
        }

        private static void RegisterKeyVault<T>(this ContainerBuilder builder)
        {
            builder.Register(c => KeyVaultProxy<T>.Create(c.Resolve<IKeyVaultConfiguration>()))
                   .As<T>()
                   .SingleInstance();
        }
    }
}