using System;
using Autofac;
using Serilog.Core;
using WX.B2C.User.Verification.Infrastructure.Common.Logging;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    internal static class LoggingModuleExtensions
    {
        public static ContainerBuilder RegisterLogging(this ContainerBuilder builder, bool useLogSwitcher)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterModule<LoggingModule>();
            if (useLogSwitcher)
                builder.RegisterType<HostLogLevelSwitcher>().As<LoggingLevelSwitch>().SingleInstance();

            return builder;
        }
    }
}