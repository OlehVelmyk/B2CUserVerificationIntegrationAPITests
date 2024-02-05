using System;
using System.Linq;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.EventPublisher.Mappers;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;
using WX.Logging.Autofac;
using WX.Messaging.EventHub.Interfaces;
using WX.Messaging.EventHub.Models;
using WX.Messaging.Publisher.Autofac;
using WX.Messaging.Stub.Autofac;

namespace WX.B2C.User.Verification.EventPublisher.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterEventPublisher(this ContainerBuilder builder, 
                                                              Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterEventHubPublishingSupport(GetEventHubConfig, configurationBuilder =>
                configurationBuilder.WithStub(shouldUseStub));

            builder.RegisterType<InnerEventMapper>()
                   .As<IInnerEventMapper>()
                   .SingleInstance();

            builder.RegisterType<IntegrationEventMapper>()
                   .As<IIntegrationEventMapper>()
                   .SingleInstance();

            builder.RegisterType<VerificationDetailsDtoMapper>()
                   .As<IVerificationDetailsDtoMapper>()
                   .SingleInstance();

            builder.RegisterType<EventDataFiller>()
                   .As<IEventDataFiller>()
                   .SingleInstance();

            builder.RegisterType<EventMapperFactory>()
                   .As<IEventMapperFactory>()
                   .SingleInstance();

            builder.RegisterType<EventPublisher>()
                   .As<IEventPublisher>()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<CollectionStepEventMapper>()
                   .As<ICollectionStepEventMapper>()
                   .SingleInstance();

            return builder;
        }

        private static IEventHubConfig GetEventHubConfig(IComponentContext context)
        {
            var appConfig = context.Resolve<IAppConfig>();
            return new EventHubConfig
            {
                EventHubConnectionStrings = appConfig.EventHubNameSpaceConnectionStrings
                                                     .Select(x => x.Value.UnSecure())
                                                     .ToArray(),
                StorageConnectionString = appConfig.StorageConnectionString.UnSecure(),
                ServiceName = appConfig.ServiceName,
                PublicKeyProvider = () => appConfig.EventHubPublicKey.UnSecure(),
                //Private key is not needed for listener, but in listener host we also have publisher, so we need private key.
                PrivateKeyProvider = () => appConfig.EventHubPrivateKey.UnSecure()
            };
        }
    }
}
