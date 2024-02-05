using Autofac;
using Microsoft.AspNetCore.Builder;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Metrics
{
    public static class MetricsReportingExtensions
    {
        public static void RegisterMetricsReporting(this ContainerBuilder builder)
        {
            builder.RegisterType<MetricsReportingMiddleware>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// DO NOT FORGET to call <see cref="RegisterMetricsReporting"/>
        /// </summary>
        public static IApplicationBuilder UseMetricsReporting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MetricsReportingMiddleware>();
        }
    }
}