using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Commands.StorageQueue;
using WX.Commands.StorageQueue.IoC;

namespace WX.B2C.User.Verification.Listener.Commands.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterCommandListener(this ContainerBuilder builder)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterCommandProcessors(processorsBuilder => processorsBuilder
                                                                             .WithStorageQueueConfiguration(BuildConfiguration)
                                                                             .WithCommandHandlerResolverFactory<ServiceCommandHandlerResolverFactory>()
                                                                             .SequentialProcessing(CommandQueueNameResolver.Get()));

            builder.Populate(serviceCollection);

            builder.RegisterType<CommandHandlerResolver>()
                   .As<ICommandHandlerResolver>()
                   .SingleInstance();

            return builder;
        }

        private static void BuildConfiguration(IServiceProvider provider, IStorageQueueProcessorConfigurationBuilder configurationBuilder)
        {
            var appConfig = provider.GetService<IAppConfig>();

            configurationBuilder.DegreeOfParallelism = 10;
            configurationBuilder.EnableDeadLetterQueue = true;
            configurationBuilder.StorageConnectionString = appConfig.CommandsQueueConnectionString;
        }
    }
}