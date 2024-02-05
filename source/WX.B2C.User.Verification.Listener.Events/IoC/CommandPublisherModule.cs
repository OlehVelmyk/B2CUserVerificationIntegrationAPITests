using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Listener.Events.Middleware;
using WX.Commands;
using WX.Commands.StorageQueue;
using WX.Commands.StorageQueue.Factories;
using WX.Commands.StorageQueue.IoC;

namespace WX.B2C.User.Verification.Listener.Events.IoC
{
    public static class CommandPublisherModule
    {
        public static void RegisterCommandsPublisher(this ContainerBuilder builder)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterCommandPublishing();

            builder.Register(context =>
                   {
                       var appConfig = context.Resolve<IAppConfig>();
                       return new StorageQueueConfiguration(appConfig.CommandsQueueConnectionString,
                                                            appConfig.CommandsQueueName);
                   })
                   .AsSelf()
                   .SingleInstance();

            builder.Register(context =>
                   {
                       var serviceProvider = context.Resolve<IServiceProvider>();
                       var config = context.Resolve<StorageQueueConfiguration>();
                       return StorageQueueCommandsPublisherFactory.Create(serviceProvider, config);
                   })
                   .As<ICommandsPublisher>()
                   .SingleInstance();

            builder.Populate(serviceCollection);
            builder.RegisterType<CommandOperationContextDecorator>().AsSelf().SingleInstance();
            builder.RegisterDecorator<CommandOperationContextDecorator, ICommandsPublisher>();
        }

    }
}
