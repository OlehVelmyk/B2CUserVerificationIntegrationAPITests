using System;
using Autofac;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterRemoting(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<OperationContextScopeFactory>()
                   .As<IOperationContextScopeFactory>()
                   .SingleInstance();

            builder.RegisterType<OperationContextProvider>()
                   .As<IOperationContextProvider>()
                   .As<IOperationContextSetter>()
                   .SingleInstance();

            return builder;
        }
    }
}
