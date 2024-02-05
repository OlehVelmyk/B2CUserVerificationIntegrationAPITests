using System;
using Autofac;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.EventPublisher.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.PassFort.IoC;

namespace WX.B2C.User.Verification.Actor.IoC
{
    internal class ActorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterInfrastructure()
                .RegisterSystemClock()
                .RegisterExceptionHandler(OnException)
                .RegisterDataAccess()
                .RegisterBlobStorage()
                .RegisterEventPublisher(ShouldUseStub)
                .RegisterCoreServices()
                .RegisterOnfidoGateway()
                .RegisterPassFortGateway();
        }
        private static bool ShouldUseStub(IComponentContext context)
        {
            var config = context.Resolve<IAppConfig>();
            return config.IsLocal;
        }

        private static void OnException(Exception exception) =>
            ActorEventSource.Current.ActorHostInitializationFailed(exception.ToString());
    }
}