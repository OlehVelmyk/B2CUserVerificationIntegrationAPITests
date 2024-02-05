using Autofac;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Internal.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Onfido.IoC;

namespace WX.B2C.User.Verification.Api.Internal.IoC
{
    internal class InternalApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterInternalFacade()
                .RegisterActorClients()
                .RegisterDataAccess()
                .RegisterFileProvider()
                .RegisterBlobStorage()
                .RegisterSystemClock()
                .RegisterConfigurations()
                .RegisterOnfidoGateway()
                .RegisterServices()
                .RegisterCommonServices();

        }
    }
}