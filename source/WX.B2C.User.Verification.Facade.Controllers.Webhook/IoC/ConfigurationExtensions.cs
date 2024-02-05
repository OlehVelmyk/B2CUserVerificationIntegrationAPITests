using System;
using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Services;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterFacade(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Register(c =>
                   {
                       var keyVault = c.Resolve<IPassFortKeyVault>();
                       return new SecretValidator(keyVault.PassFortSecret);
                   })
                   .As<ISecretValidator>()
                   .SingleInstance();

            builder.AddMediatR(typeof(CompletePassFortCheck).Assembly);
            builder.RegisterMappers();

            return builder;
        }

        private static void RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<PassFortWebhookMapper>()
                   .As<IPassFortWebhookMapper>()
                   .SingleInstance();
        }
    }
}
