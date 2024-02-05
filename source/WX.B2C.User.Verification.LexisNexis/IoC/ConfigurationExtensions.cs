using System;
using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.LexisNexis.Extractors;
using WX.B2C.User.Verification.LexisNexis.Factories;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.LexisNexis.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterBridgerCredentialsService(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterBridgerClient();

            builder.RegisterType<BridgerCredentialsService>()
                   .As<IBridgerCredentialsService>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterLexisNexisProvider(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterRdpClient()
                   .RegisterBridgerClient()
                   .RegisterCheckProviders(shouldUseStub);

            builder.RegisterType<CheckRequestMapper>()
                   .As<ICheckRequestMapper>()
                   .SingleInstance();

            builder.RegisterType<BridgerCredentialsService>()
                   .As<IBridgerCredentialsService>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterLexisNexisExtractors(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<RiskScreeningCheckOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.LexisNexis);
                       m.For(p => p.CheckType, CheckType.RiskListsScreening);
                   });

            builder.RegisterType<FraudScreeningCheckOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.LexisNexis);
                       m.For(p => p.CheckType, CheckType.FraudScreening);
                   });

            return builder;
        }

        private static ContainerBuilder RegisterRdpClient(this ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var keyVault = c.Resolve<ILexisNexisRdpKeyVault>();
                var appConfig = c.Resolve<IAppConfig>();
                return new RdpApiClientSettings
                {
                    Host = new Uri(appConfig.LexisNexisRdpHost),
                    ProxyHost = new Uri(appConfig.LexisNexisRdpProxy),
                    Mode = appConfig.LexisNexisMode,
                    ApiKeyId = keyVault.ApiKeyId.UnSecure(),
                    ApiSecretKey = keyVault.ApiSecretKey.UnSecure(),
                    AccountId = keyVault.AccountId.UnSecure(),
                    WorkflowName = keyVault.WorkflowName.UnSecure()
                };
            });

            builder.RegisterType<RdpApiClientFactory>()
                   .As<IRdpApiClientFactory>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterBridgerClient(this ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var keyVault = c.Resolve<ILexisNexisBridgerKeyVault>();
                var appConfig = c.Resolve<IAppConfig>();
                return new BridgerApiClientSettings
                {
                    BaseUri = new Uri(appConfig.LexisNexisBridgerServiceEndpoint),
                    ClientId = keyVault.ClientId.UnSecure(),
                    RolesOrUsers = keyVault.RolesOrUsers.UnSecure(),
                    UserId = keyVault.UserId.UnSecure()
                };
            });

            builder.RegisterType<BridgerApiClientFactory>()
                   .As<IBridgerApiClientFactory>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterCheckProviders(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            builder.RegisterCheckProvider(CheckProviderType.LexisNexis, options =>
            {
                options.AddFactory<FraudScreeningCheckProviderFactory>(CheckType.FraudScreening, new PlainCheckProcessingResultFactory<LexisNexisFraudScreeningOutputData>());
                options.AddFactory<RiskScreeningCheckProviderFactory>(CheckType.RiskListsScreening, new RiskScreeningCheckProcessingResultFactory());
            }, shouldUseStub);

            return builder;
        }
    }
}
