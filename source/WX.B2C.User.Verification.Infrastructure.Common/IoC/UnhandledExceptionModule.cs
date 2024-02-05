using System;
using System.Threading.Tasks;
using Autofac;
using Serilog;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    internal class UnhandledExceptionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnhandledExceptionLogger>()
                   .As<UnhandledExceptionLogger>()
                   .SingleInstance();

            builder.RegisterBuildCallback(StartUnhandledExceptionLogger);
        }

        private static void StartUnhandledExceptionLogger(IContainer container) =>
            container.Resolve<UnhandledExceptionLogger>();
    }

    internal class UnhandledExceptionLogger
    {
        private readonly ILogger _logger;

        public UnhandledExceptionLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var aggregateException = e.Exception;
            if (aggregateException == null)
                return;
            if (e.Observed)
                return;

            _logger.Error(e.Exception, "Unhandled exception detected");
            e.SetObserved();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                _logger.Error(ex, "Unhandled exception detected");
        }
    }
}