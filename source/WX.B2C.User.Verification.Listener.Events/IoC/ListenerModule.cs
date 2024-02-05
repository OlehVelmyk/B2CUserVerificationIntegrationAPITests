using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Client.Notification.IoC;
using WX.B2C.User.Verification.Configuration.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.EventHandlers.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Provider.Services.IoC;
using WX.B2C.User.Verification.EmailSender.IoC;
using WX.B2C.User.Verification.LexisNexis.IoC;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.PassFort.IoC;

namespace WX.B2C.User.Verification.Listener.Events.IoC
{
    internal class ListenerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceFabricSupport(OnException);
            builder.RegisterStatelessService<EventListenerService>(ServiceDefinitions.ServiceTypeName);

            builder
                .RegisterSystemClock()
                .RegisterCommonServices()
                .RegisterInfrastructure()
                .RegisterConfigurationSeeds()
                .RegisterMetrics()
                .RegisterExceptionHandler(OnException)
                .RegisterActorClients()
                .RegisterDataAccess()
                .RegisterCredentialsProviders()
                .RegisterClientNotification()
                .RegisterJobServiceClient()
                .RegisterCheckProviderClient()
                .RegisterEventHandlers()
                .RegisterEventListener(ShouldUseStub)
                .RegisterEventHandlerServices(ShouldUseStub)
                .RegisterEmailSender()
                .RegisterUserEmailService()
                .RegisterBlobStorage()
                .RegisterConfigurations()
                .RegisterSystemExtractors()
                .RegisterLexisNexisExtractors()
                .RegisterOnfidoExtractors()
                .RegisterPassFortExtractors();
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
