using System;
using Autofac;
using Serilog.Events;
using WX.B2C.Client.Notification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Logging.Autofac;
using static WX.B2C.Client.Notification.Commands.NotificationClientCommandsPublisherFactory;

namespace WX.B2C.User.Verification.Client.Notification.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterClientNotification(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Register(CreatePublisher)
                   .As<INotificationClientCommandsPublisher>()
                   .SingleInstance();

            builder.RegisterType<UserNotificationService>()
                   .As<IUserNotificationService>()
                   .SingleInstance()
                   .UseCallLogger(LogEventLevel.Information);

            return builder;
        }

        private static INotificationClientCommandsPublisher CreatePublisher(IComponentContext context)
        {
            var keyVault = context.Resolve<IB2CUserVerificationKeyVault>();
            var connectionString = keyVault.B2CStorageConnectionString;
            var serviceProvider = context.Resolve<IServiceProvider>();

            return CreateNotificationClientCommandsPublisher(serviceProvider, connectionString, LogEventLevel.Debug);
        }
    }
}