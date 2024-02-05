using Autofac;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;

namespace WX.B2C.User.Verification.Api.Webhook.IoC
{
    internal class WebhookModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterSystemClock()
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterDataAccess()
                .RegisterCommonServices()
                .RegisterActorClients()
                .RegisterFacade();
        }
    }
}
