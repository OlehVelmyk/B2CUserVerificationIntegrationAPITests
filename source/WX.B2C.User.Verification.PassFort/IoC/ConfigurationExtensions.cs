using System;
using System.Collections.Generic;
using System.Security;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts.Module;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Factories;
using WX.B2C.User.Verification.PassFort.Mappers;
using WX.B2C.User.Verification.PassFort.Providers;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;
using WX.Core.TypeExtensions;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.PassFort.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterPassFortGateway(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            RegisterPassFortClient(builder);
            RegisterProfileUpdater(builder);
            RegisterMappers(builder);

            builder.RegisterType<PassFortGateway>()
                   .As<IPassFortGateway>()
                   .SingleInstance();

            builder.RegisterType<PassFortTagGateway>()
                   .As<IPassFortTagGateway>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterPassFortProvider(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            RegisterPassFortClient(builder);
            RegisterProfileUpdater(builder);
            RegisterPassFortCheckProvider(builder, shouldUseStub);
            RegisterMappers(builder);

            return builder;
        }

        public static void RegisterPassFortExtractors(this ContainerBuilder builder)
        {
            builder.RegisterType<RiskScreeningCheckOutputDataExtractor>()
                   .As<ICheckOutputDataExtractor>()
                   .WithMetadata<CheckProviderMetadata>(m =>
                   {
                       m.For(p => p.ProviderType, CheckProviderType.PassFort);
                       m.For(p => p.CheckType, CheckType.RiskListsScreening);
                   })
                   .SingleInstance();
        }

        private static void RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<IndividualDataMapper>()
                   .As<IIndividualDataMapper>()
                   .SingleInstance();

            builder.RegisterType<PassFortEnumerationsMapper>()
                   .As<IPassFortEnumerationsMapper>()
                   .SingleInstance();

            builder.RegisterType<PassFortApplicationMapper>()
                   .As<IPassFortApplicationMapper>()
                   .SingleInstance();
        }

        private static void RegisterProfileUpdater(this ContainerBuilder builder)
        {
            builder.RegisterType<ProfileDataPatchMapper>()
                   .As<IProfileDataPatchMapper>()
                   .SingleInstance();
            
            builder.RegisterType<PatchDataComparer>()
                   .As<IPatchDataComparer>()
                   .SingleInstance();

            builder.RegisterType<PassFortProfilePatcher>()
                   .As<IPassFortProfilePatcher>()
                   .SingleInstance();

            builder.RegisterType<PassFortProfileUpdater>()
                   .As<IPassFortProfileUpdater>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);
        }

        private static void RegisterPassFortCheckProvider(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            builder.RegisterCheckProvider(CheckProviderType.PassFort, options =>
            {
                options.AddFactory<RiskScreeningCheckProviderFactory>(CheckType.RiskListsScreening);
            }, shouldUseStub);
        }

        private static void RegisterPassFortClient(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var appConfig = c.Resolve<IAppConfig>();
                var keyVault = c.Resolve<IPassFortKeyVault>();
                return new PassFortApiSettings
                {
                    ApiUri = new Uri(appConfig.PassFortApiUrl),
                    ApiKey = keyVault.PassFortApiKey.UnSecure()
                };
            });

            builder.Register(c =>
                   {
                       var keyVault = c.Resolve<IPassFortKeyVault>();

                       return new PassFortApplicationProductProvider(
                           new Dictionary<string, SecureString>
                           {
                               ["APAC"] = keyVault.ApacApplicationProductId,
                               ["RoW"] = keyVault.RowApplicationProductId,
                               ["US"] = null
                           }
                       );
                   })
                   .As<IPassFortApplicationProductProvider>()
                   .SingleInstance();

            builder.RegisterType<PassFortProfileFactory>()
                   .Keyed<IExternalProfileFactory>(ExternalProviderType.PassFort)
                   .SingleInstance();

            builder.RegisterType<PassFortApiClientFactory>()
                   .As<IPassFortApiClientFactory>()
                   .SingleInstance();
            
            builder.RegisterType<PassFortPolicyFactory>()
                   .As<IPassFortPolicyFactory>()
                   .SingleInstance();
        }
    }
}