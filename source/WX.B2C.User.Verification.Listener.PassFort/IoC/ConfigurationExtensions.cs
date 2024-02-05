using System;
using Autofac;
using Serilog.Events;
using WX.B2C.User.Verification.Listener.PassFort.EventHandlers;
using WX.B2C.User.Verification.Listener.PassFort.Services;
using WX.Logging.Autofac;
using WX.Messaging.Subscriber.Autofac;

namespace WX.B2C.User.Verification.Listener.PassFort.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterEventHandlers(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<EventHandlingContext>().InstancePerLifetimeScope();

            builder.RegisterEventHandler<PassFortSynchronizationEventHandler>();
            builder.RegisterEventHandler<PassFortTagsEventHandler>();

            return builder;
        }

        public static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ExternalProfileAdapter>()
                   .As<IExternalProfileAdapter>()
                   .SingleInstance();

            builder.RegisterType<PassFortSynchronizationService>()
                   .As<IPassFortSynchronizationService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<PassFortTagService>()
                   .As<IPassFortTagService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            return builder;
        }
    }
}