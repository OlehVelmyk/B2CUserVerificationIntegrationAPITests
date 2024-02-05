using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.EventPublisher.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.IoC;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Remoting.IoC;
using WX.B2C.User.Verification.LexisNexis.IoC;
using WX.B2C.User.Verification.Onfido.IoC;
using WX.B2C.User.Verification.Infrastructure.Communication.IoC;
using WX.B2C.User.Verification.IpStack.IoC;
using WX.B2C.User.Verification.PassFort.IoC;

namespace WX.B2C.User.Verification.Worker.Jobs.IoC
{
    internal class JobsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterServiceFabricSupport();
            builder.RegisterStatelessService<JobsService>(ServiceDefinitions.ServiceTypeName);

            builder
                .RegisterInfrastructure()
                .RegisterMetrics()
                .RegisterExceptionHandler(OnException)
                .RegisterConfigurations()
                .RegisterDataAccess()
                .RegisterDbFactories()
                .RegisterRemoting()
                .RegisterQuartzScheduler()
                .RegisterJobServices()
                .RegisterJobValidators()
                .RegisterJobRepositories()
                .RegisterJobDataAggregationServices()
                .RegisterJobBuilders()
                .RegisterTriggerKeyFactories()
                .RegisterJobKeyFactories()
                .RegisterJobProviders()
                .RegisterJobs()
                .RegisterJobConfigs()
                .RegisterCredentialsProviders()
                .RegisterBridgerCredentialsService()
                .RegisterPassFortGateway()
                .RegisterIpStackGateway()
                .RegisterBlobStorage()
                .RegisterSystemClock()
                .RegisterCommonServices()
                .RegisterOnfidoGateway()
                .RegisterOnfidoThrottler()
                .RegisterActorClients()
                .RegisterCheckProviderClient()
                .RegisterEventPublisher(ShouldUseStub);
        }

        private static bool ShouldUseStub(IComponentContext context)
        {
            var config = context.Resolve<IAppConfig>();
            return config.IsLocal;
        }

        private static void OnException(Exception exception) =>
            ServiceEventSource.Current.ServiceHostInitializationFailed(exception.ToString());
    }
}
