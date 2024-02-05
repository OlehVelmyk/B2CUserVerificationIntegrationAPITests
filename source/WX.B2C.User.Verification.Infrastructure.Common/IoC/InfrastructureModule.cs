using System.Fabric;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Core.TypeExtensions;
using WX.KeyVault;
using WX.Logging.Autofac;
using WX.ServiceFabric;
using WX.ServiceFabric.Autofac;
using WX.ServiceFabric.Interfaces;
using WX.ServiceFabric.Telemetry;
using WX.ServiceFabric.Telemetry.Autofac;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    internal class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterWxServiceFabric();

            builder.Register(c => FabricRuntime.GetActivationContext())
                   .As<ICodePackageActivationContext>()
                   .SingleInstance();

            builder.RegisterType<FabricHostSettingsProvider>()
                   .As<IHostSettingsProvider>()
                   .SingleInstance();

            builder.RegisterType<FabricAddressResolver>()
                   .As<IFabricAddressResolver>()
                   .UseCallLogger(LogEventLevel.Information)
                   .SingleInstance();

            builder.RegisterType<WxEnvironment>()
                   .As<IWxEnvironment>()
                   .SingleInstance();

            builder.RegisterType<FabricCallContext>()
                   .As<ICallContext>()
                   .SingleInstance();

            builder.RegisterType<Rc2EncryptProvider>()
                   .As<IEncryptProvider>()
                   .SingleInstance();

            builder.RegisterType<IdempotentGuidGenerator>()
                   .As<IIdempotentGuidGenerator>()
                   .SingleInstance();

            builder.Register(c =>
                       AppConfigFactory.Create(() =>
                               c.Resolve<IWxEnvironment>()
                                .Environment,
                           c.Resolve<IB2CUserVerificationKeyVault>,
                           c.Resolve<IMessagingKeyVault>,
                           c.Resolve<IConfigurationKeyVault>,
                           c.Resolve<IHostSettingsProvider>))
                   .As<IAppConfig>()
                   .SingleInstance();

            builder.RegisterTelemetry(c =>
            {
                var config = c.Resolve<IAppConfig>();
                return config.AppInsightsInstrumentationKey.UnSecure();
            }, TelemetryComponentProviderType.Default);

            DependencyInjectionIssueDetector.CheckAfterBuild(builder);
        }
    }
}
