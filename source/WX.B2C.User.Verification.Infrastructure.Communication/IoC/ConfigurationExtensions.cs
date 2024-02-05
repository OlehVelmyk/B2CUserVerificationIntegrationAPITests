using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Infrastructure.Communication.Clients;

namespace WX.B2C.User.Verification.Infrastructure.Communication.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterActorClients(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ApplicationActorClient>()
                   .As<IApplicationServiceClient>()
                   .As<IApplicationService>();
            builder.RegisterType<ProfileActorClient>()
                   .As<IProfileService>();
            builder.RegisterType<DocumentActorClient>()
                   .As<IDocumentService>();
            builder.RegisterType<FileActorClient>()
                   .As<IFileService>();
            builder.RegisterType<TaskActorClient>()
                   .As<ITaskServiceClient>()
                   .As<ITaskService>();
            builder.RegisterType<CollectionStepActorClient>()
                   .As<ICollectionStepServiceClient>()
                   .As<ICollectionStepService>();
            builder.RegisterType<CheckActorClient>()
                   .As<ICheckServiceClient>()
                   .As<ICheckService>();
            builder.RegisterType<ExternalProfileActorClient>()
                   .As<IExternalProfileProvider>();
            builder.RegisterType<TriggerActorClient>()
                   .As<ITriggerService>();

            builder.RegisterType<ServiceClientFactory>().As<IServiceClientFactory>().SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterJobServiceClient(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<JobServiceClient>()
                   .As<IJobService>();

            return builder;
        }
        public static ContainerBuilder RegisterCheckProviderClient(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<CheckProviderServiceClient>()
                   .As<ICheckProviderService>();

            return builder;
        }

    }
}