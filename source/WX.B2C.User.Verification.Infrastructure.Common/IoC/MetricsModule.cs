using Autofac;
using Prometheus;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Common.Monitoring;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    internal class MetricsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
                   {
                       var logger = context.Resolve<ILogger>();
                       var hostSettingsProvider = context.Resolve<IHostSettingsProvider>();
                       var port = hostSettingsProvider.GetSetting(HostSettingsKey.MetricsPort);
                       var host = hostSettingsProvider.GetSetting(HostSettingsKey.ApplicationHost);
                       logger.Information("Starting metric server for host:{Host} on port:{Port}", host, port);
                       var server = MetricServerFactory.Start(int.Parse(port));
                       logger.Information("Started metric server for host:{Host} on port:{Port}", host, port);
                       return server;
                   })
                   .As<IMetricServer>()
                   .SingleInstance();
                
            builder.RegisterBuildCallback(StartMetricService);
            
            builder.RegisterType<PrometheusMetricsFactory>().AsSelf().SingleInstance();
            builder.RegisterType<PrometheusMetricsDashboard>().As<IPrometheusMetricsDashboard>().SingleInstance();
            builder.RegisterType<PrometheusMetricsLogger>().As<IMetricsLogger>().SingleInstance();
        }

        private void StartMetricService(IContainer container) =>
            container.Resolve<IMetricServer>();
    }
}