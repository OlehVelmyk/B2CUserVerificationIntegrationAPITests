using Autofac;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Admin.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.LexisNexis.IoC;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.Survey.IoC;

namespace WX.B2C.User.Verification.Api.Admin.IoC
{
    internal class AdminApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterInfrastructure()
                .RegisterSystemClock()
                .RegisterMetrics()
                .RegisterDataAccess()
                .RegisterBlobStorage()
                .RegisterCommonServices()
                .RegisterAdminServices()
                .RegisterExternalLinkProvider()
                .RegisterFileProvider()
                .RegisterOnfidoGateway()
                .RegisterAdminFacade()
                .RegisterBridgerCredentialsService()
                .RegisterCredentialsProviders()
                .RegisterActorClients()
                .RegisterJobServiceClient()
                .RegisterSurveyGateway(ShouldUseStub)
                .RegisterConfigurations();
        }

        private static bool ShouldUseStub(IComponentContext context)
        {
            var config = context.Resolve<IAppConfig>();
            return config.IsLocal;
        }
    }
}
