using System;
using Autofac;
using Serilog;
using WX.B2C.User.Verification.Facade.Commands.Middleware;

namespace WX.B2C.User.Verification.Facade.Commands.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterCommandHandlers(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Register<ApplicationCommandHandler>()
                   .Register<TaskCommandHandler>()
                   .Register<CheckCommandHandler>()
                   .Register<CollectionStepCommandHandler>()
                   .Register<TriggerCommandHandler>()
                   .Register<ExternalProfileCommandHandler>();

            builder.RegisterMiddleware();

            return builder;
        }

        internal static ContainerBuilder RegisterMiddleware(this ContainerBuilder builder)
        {
            builder.Register(context => new LoggingDecorator(context.Resolve<ILogger>()))
                   .As<ICommandHandlerDecorator>();

            builder.RegisterType<OperationContextDecorator>()
                   .As<ICommandHandlerDecorator>();

            return builder;
        }

        internal static ContainerBuilder Register<THandler>(this ContainerBuilder builder)
        {
            builder.RegisterType<THandler>().AsImplementedInterfaces();
            return builder;
        }
    }
}