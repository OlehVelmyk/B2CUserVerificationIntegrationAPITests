using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.EmailSender.Mappers;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.EmailSender.Commands.IoC;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.EmailSender.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterEmailSender(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var serviceCollection = new ServiceCollection();

            serviceCollection.RegisterEmailSenderClient(provider =>
            {
                var keyVault = provider.GetService<IB2CUserVerificationKeyVault>();
                return new EmailSenderOptions(keyVault.B2CStorageConnectionString);
            }, LogEventLevel.Information);

            builder.Populate(serviceCollection);

            builder.RegisterType<UserEmailProvider>()
                .As<IUserEmailProvider>()
                .SingleInstance()
                .UseCallLogger(LogEventLevel.Information);

            builder.RegisterType<UserEmailSenderMapper>()
                .As<IUserEmailSenderMapper>()
                .SingleInstance();

            return builder;
        }
    }
}
