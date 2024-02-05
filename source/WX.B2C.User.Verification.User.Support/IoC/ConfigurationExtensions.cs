using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using WX.B2C.User.Support.Commands.Configuration;
using WX.B2C.User.Support.Commands.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.KeyVault;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.User.Support.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterUserSupportGateway(this ContainerBuilder builder, 
                                                                  Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                   .RegisterClient()
                   .RegisterSender(shouldUseStub);
        }

        private static ContainerBuilder RegisterClient(this ContainerBuilder builder)
        {
            var serviceProvider = new ServiceCollection();
            serviceProvider.RegisterUserSupportClient(
                provider => KeyVaultProxy<IUserSupportKeyVault>.Create(provider.GetRequiredService<KeyVaultConfiguration>()),
                Constants.ApplicationName);
            
            builder.Populate(serviceProvider);

            return builder;
        }
        
        private static ContainerBuilder RegisterSender(this ContainerBuilder builder, Predicate<IComponentContext> shouldUseStub)
        {
            builder.RegisterType<TicketSender>().AsSelf();
            builder.RegisterType<TicketSenderStub>().AsSelf();

            builder.Register(context => shouldUseStub(context)
                                 ? (ITicketSender) context.Resolve<TicketSenderStub>()
                                 : context.Resolve<TicketSender>())
                   .As<ITicketSender>()
                   .UseCallLogger(LogEventLevel.Information)
                   .SingleInstance();
            
            return builder;
        }
    }
}
