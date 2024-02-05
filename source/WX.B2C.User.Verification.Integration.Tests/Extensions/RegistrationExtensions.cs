using Autofac;
using Microsoft.Extensions.Configuration;
using WX.B2C.User.Verification.Onfido;
using WX.B2C.User.Verification.Onfido.Client;

namespace WX.B2C.User.Verification.Integration.Tests.Extensions
{
    internal static class RegistrationExtensions
    {
        public static ContainerBuilder RegisterOnfidoClient(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>().GetSection("Onfido");
                return new OnfidoClientSettings(configuration.GetValue<string>("ApiUrl"), configuration.GetValue<string>("ApiToken"));
            }).SingleInstance();

            builder.RegisterType<OnfidoPolicyFactory>().As<IOnfidoPolicyFactory>().SingleInstance();
            builder.RegisterType<OnfidoApiClientFactory>().As<IOnfidoApiClientFactory>().SingleInstance();

            return builder;
        }

    }
}
