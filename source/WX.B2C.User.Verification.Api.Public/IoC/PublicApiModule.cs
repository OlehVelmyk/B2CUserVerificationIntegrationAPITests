using Autofac;
using WX.B2C.User.Verification.B2C.Survey.IoC;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Public.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Onfido.IoC;

namespace WX.B2C.User.Verification.Api.Public.IoC
{
    internal class PublicApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterSystemClock()
                .RegisterCommonServices()
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterDataAccess()
                .RegisterPublicFacade()
                .RegisterPublicServices()
                .RegisterBlobStorage()
                .RegisterConfigurations()
                .RegisterB2CSurveyGateway(ShouldUseStub)
                .RegisterFileProvider()
                .RegisterOnfidoGateway()
                .RegisterActorClients();
        }

        private static bool ShouldUseStub(IComponentContext context)
        {
            var config = context.Resolve<IAppConfig>();
            return config.IsLocal;
        }
    }
}
