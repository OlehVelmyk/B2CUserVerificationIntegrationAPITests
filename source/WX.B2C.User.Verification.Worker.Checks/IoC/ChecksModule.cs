using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using WX.B2C.User.Verification.B2C.Survey.IoC;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.IpStack.IoC;
using WX.B2C.User.Verification.LexisNexis.IoC;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.PassFort.IoC;
using WX.B2C.User.Verification.Provider.Services.IoC;
using WX.B2C.User.Verification.Worker.Checks;

namespace WX.B2C.User.Verification.Providers.IoC
{
    internal class ChecksModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceFabricSupport();
            builder.RegisterStatelessService<ChecksWorker>(ServiceDefinitions.ServiceTypeName);

            builder
                .RegisterSystemClock()
                .RegisterInfrastructure()
                .RegisterExceptionHandler(OnException)
                .RegisterConfigurations()
                .RegisterBlobStorage()
                .RegisterDataAccess()
                .RegisterCommonServices()
                .RegisterPassFortProvider(ShouldUseStub)
                .RegisterOnfidoProvider(ShouldUseStub)
                .RegisterLexisNexisProvider(ShouldUseStub)
                .RegisterSystemCheckProvider(ShouldUseStub)
                .RegisterSandboxDecorators()
                .RegisterCredentialsProviders()
                .RegisterB2CSurveyGateway(ShouldUseStub)
                .RegisterIpStackGateway()
                .RegisterCheckProviderService()
                .RegisterActorClients();
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
