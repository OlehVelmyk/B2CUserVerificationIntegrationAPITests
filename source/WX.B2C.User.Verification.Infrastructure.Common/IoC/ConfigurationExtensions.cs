using System;
using Autofac;
using Serilog;
using WX.B2C.User.Verification.Configuration.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.Monitoring;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Remoting.IoC;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterServiceRunningInfrastructure(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterConfiguration();
            builder.RegisterLogging(false);
            builder.RegisterRemoting();

            return builder;
        }

        public static ContainerBuilder RegisterInfrastructure(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<UnhandledExceptionModule>();

            builder.RegisterLogging(true);
            builder.RegisterConfiguration();
            builder.RegisterRemoting();
            builder.RegisterMetricsStub();

            return builder;
        }

        public static ContainerBuilder RegisterMetrics(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterModule<MetricsModule>();

            return builder;
        }

        public static ContainerBuilder RegisterCredentialsProviders(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<BridgerCredentialsProvider>()
                   .As<IBridgerCredentialsProvider>()
                   .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterExceptionHandler(this ContainerBuilder builder, Action<Exception> onIocException)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            IContainer container = null;
            builder.Register(context => container).SingleInstance();
            builder.RegisterBuildCallback(c =>
            {
                container = c;
                var logger = c.Resolve<ILogger>();

                container.ResolveOperationBeginning += (s, e) =>
                {
                    e.ResolveOperation.CurrentOperationEnding += (s1, e1) =>
                    {
                        if (e1.Exception != null)
                        {
                            onIocException?.Invoke(e1.Exception);
                            logger.Error(e1.Exception, "Error in Autofac");
                        }
                    };
                };
            });

            return builder;
        }

        private static ContainerBuilder RegisterMetricsStub(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<MetricsLoggerStub>().As<IMetricsLogger>().SingleInstance();

            return builder;
        }
    }
}
