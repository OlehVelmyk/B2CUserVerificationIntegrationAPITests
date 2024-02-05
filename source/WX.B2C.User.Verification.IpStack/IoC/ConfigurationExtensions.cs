using System;
using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.IpStack.Mappers;

namespace WX.B2C.User.Verification.IpStack.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterIpStackGateway(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<IpAddressLocationMapper>()
                   .As<IIpAddressLocationMapper>()
                   .SingleInstance();

            builder.RegisterType<IpStackApiClientFactory>()
                   .As<IIpStackApiClientFactory>()
                   .SingleInstance();

            builder.RegisterType<IpStackGateway>()
                   .As<IIpAddressLocationProvider>()
                   .SingleInstance();

            return builder;
        }
    }
}
