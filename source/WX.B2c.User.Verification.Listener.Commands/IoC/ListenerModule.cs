using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.Core.Services.RequiredData;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.Commands.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.User.Support.IoC;

namespace WX.B2C.User.Verification.Listener.Commands.IoC
{
    internal class ListenerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceFabricSupport(OnException);
            builder.RegisterStatelessService<CommandListenerService>(ServiceDefinitions.ServiceTypeName);

            builder.RegisterType<ProfileProviderFactory>().As<IProfileProviderFactory>().SingleInstance();

            builder
                .RegisterSystemClock()
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterExceptionHandler(OnException)
                .RegisterActorClients()
                .RegisterCommandListener()
                .RegisterTicketSender()
                .RegisterUserSupportGateway(ShouldUseStub)
                .RegisterDataAccess()
                .RegisterCommonServices()
                .RegisterCommandHandlers();
        }

        private static void OnException(Exception exception) =>
            ServiceEventSource.Current.ServiceHostInitializationFailed(exception.ToString());

        private static bool ShouldUseStub(IComponentContext context)
        {
            var env = context.Resolve<IAppConfig>();
            return env.IsLocal;
        }
    }
}