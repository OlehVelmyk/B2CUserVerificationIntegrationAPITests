using System;
using Autofac;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Provider.Contracts.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterCheckProvider(this ContainerBuilder builder, 
                                                             CheckProviderType providerType, 
                                                             Action<CheckProviderOptions> configure, 
                                                             Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var options = new CheckProviderOptions(providerType);
            configure(options);

            builder.RegisterInstance(options)
                   .SingleInstance()
                   .AsSelf();

            if (options.ComplexFactory != null)
            {
                builder.RegisterType(options.ComplexFactory)
                       .As<ICheckProviderFactory>()
                       .WithMetadata<CheckProviderMetadata>(m =>
                       {
                           m.For(p => p.ProviderType, providerType);
                       })
                       .SingleInstance();
            }

            foreach (var (checkType, factory) in options.Factories)
            {
                builder.RegisterType(factory).SingleInstance();
                builder.Register(context =>
                {
                    if (shouldUseStub(context) && options.StubFactories.ContainsKey(checkType))
                    {
                        var stubFactory = options.StubFactories[checkType];
                        return new CheckProviderStubFactory(stubFactory);
                    }

                    return context.Resolve(factory);
                })
                .As<ICheckProviderFactory>()
                .WithMetadata<CheckProviderMetadata>(m =>
                {
                    m.For(p => p.ProviderType, providerType);
                    m.For(p => p.CheckType, checkType);
                })
                .SingleInstance();
            }

            return builder;
        }
    }
}
