using System;
using System.Linq;
using Autofac;
using Autofac.Integration.ServiceFabric;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.PassFort.IoC;
using WX.Core.TypeExtensions;
using WX.Messaging.EventHub.Interfaces;
using WX.Messaging.EventHub.Models;
using WX.Messaging.Stub.Autofac;
using WX.Messaging.Subscriber.Autofac;

namespace WX.B2C.User.Verification.Listener.PassFort.IoC
{
    internal class ListenerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceFabricSupport(OnException);
            builder.RegisterStatelessService<EventListenerService>(ServiceDefinitions.ServiceTypeName);

            builder.RegisterEventHubSubscriptionSupport(ResolveConfig, configurationBuilder =>
                                                            configurationBuilder.WithStub(ShouldUseStub));

            builder
                .RegisterConfigurations()
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterExceptionHandler(OnException)
                .RegisterCommonServices()
                .RegisterDataAccess()
                .RegisterPassFortGateway()
                .RegisterOnfidoGateway()
                .RegisterFileProvider()
                .RegisterBlobStorage()
                .RegisterSystemClock()
                .RegisterEventHandlers()
                .RegisterServices()
                .RegisterActorClients();
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
                ServiceName = "b2c-passfort-sync",
                PublicKeyProvider = () => appConfig.EventHubPublicKey.UnSecure(),
                //Private key is not needed for listener, but in listener host we also have publisher, so we need private key.
                PrivateKeyProvider = () => appConfig.EventHubPrivateKey.UnSecure()
            };
        }

        private static bool ShouldUseStub(IComponentContext context)
        {
            var env = context.Resolve<IAppConfig>();
            return env.IsLocal;
        }

        private static void OnException(Exception exception) =>
            ServiceEventSource.Current.ServiceHostInitializationFailed(exception.ToString());
    }
}
