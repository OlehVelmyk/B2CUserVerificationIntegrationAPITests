using System;
using System.Linq;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Automation.Services.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.Core.Services.Providers;
using WX.B2C.User.Verification.Core.Services.RequiredData;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.User.Support.IoC;
using WX.Core.TypeExtensions;
using WX.Logging.Autofac;
using WX.Messaging.EventHub.Interfaces;
using WX.Messaging.EventHub.Models;
using WX.Messaging.Stub.Autofac;
using WX.Messaging.Subscriber.Autofac;

namespace WX.B2C.User.Verification.Listener.Events.IoC
{
    internal static class ConfigurationExtensions
    {
        internal static ContainerBuilder RegisterEventListener(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (shouldUseStub == null)
                throw new ArgumentNullException(nameof(shouldUseStub));

            builder.RegisterEventHubSubscriptionSupport(ResolveConfig, configurationBuilder =>
                configurationBuilder.WithStub(shouldUseStub));

            return builder;
        }

        internal static ContainerBuilder RegisterEventHandlerServices(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<AuditService>().As<IAuditService>().UseCallLogger(LogEventLevel.Information).SingleInstance();
            builder.RegisterType<ProfileProviderFactory>().As<IProfileProviderFactory>().SingleInstance();
            builder.RegisterType<PolicySelectionContextProvider>().As<IPolicySelectionContextProvider>().SingleInstance();
            builder.RegisterType<RegionActionsProvider>().As<IRegionActionsProvider>().SingleInstance();

            builder.RegisterCommandsPublisher();
            builder.RegisterAutomationServices();
            builder.RegisterTicketSender();
            builder.RegisterUserSupportGateway(shouldUseStub);

            return builder;
        }

        private static IEventHubConfig ResolveConfig(IComponentContext context)
        {
            var appConfig = context.Resolve<IAppConfig>();
            return new EventHubConfig
            {
                EventHubConnectionStrings = appConfig.EventHubNameSpaceConnectionStrings
                                                     .Select(x => x.Value.UnSecure())
                                                     .ToArray(),
                StorageConnectionString = appConfig.StorageConnectionString.UnSecure(),
                ServiceName = appConfig.ServiceName,
                PublicKeyProvider = () => appConfig.EventHubPublicKey.UnSecure(),
                //Private key is not needed for listener, but in listener host we also have publisher, so we need private key.
                PrivateKeyProvider = () => appConfig.EventHubPrivateKey.UnSecure()
            };
        }
    }
}