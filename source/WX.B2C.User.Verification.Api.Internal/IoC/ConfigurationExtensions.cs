using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Services;

namespace WX.B2C.User.Verification.Api.Internal.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<TokenService>()
                   .As<ITokenService>()
                   .SingleInstance();

            return builder;
        }
    }
}