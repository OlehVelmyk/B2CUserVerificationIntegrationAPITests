using System;
using Autofac;
using Masking.Serilog;
using Serilog.Core;
using Serilog.Events;
using WX.B2C.User.Verification.Infrastructure.Common.Logging;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;
using WX.Logging;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.Infrastructure.Common.IoC
{
    internal class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var settingsProvider = c.Resolve<IHostSettingsProvider>();
                var logLevel = settingsProvider.GetSetting(HostSettingsKey.LogLevel);
                return new LoggingLevelSwitch(Enum.Parse<LogEventLevel>(logLevel));

            }).As<LoggingLevelSwitch>().SingleInstance();

            builder.RegisterLogging((componentContext, configuration) =>
            {
                var config = componentContext.Resolve<IAppConfig>();
                var contextProvider = componentContext.Resolve<IOperationContextProvider>();
                var hostSettingProvider = componentContext.Resolve<IHostSettingsProvider>();
                var logLevelSwitcher = componentContext.Resolve<LoggingLevelSwitch>();

                var splunkUrl = config.SplunkEndpoint.UnSecure();
                var splunkToken = config.SplunkToken.UnSecure();
                configuration.ForApp(hostSettingProvider.GetSetting(HostSettingsKey.AppName))
                             .ForEnvironment(hostSettingProvider.GetSetting(HostSettingsKey.Environment))
                             .ForHost(hostSettingProvider.GetSetting(HostSettingsKey.ApplicationHost))
                             .ForVersion(hostSettingProvider.GetSetting(HostSettingsKey.Version))
                             .WithMinimumLogLevel(logLevelSwitcher)
                             .WriteToSplunk(splunkUrl, splunkToken)
                             .WriteToDebug()
                             .Enrich.FromLogContext()
                             .Enrich.With(new OperationContextEnricher(contextProvider))
                             .Enrich.With(new HostSettingEnricher(hostSettingProvider))
                             .Destructure.ByMaskingProperties(opts =>
                             {
                                 opts.PropertyNames.Add("APIKEY");
                                 opts.PropertyNames.Add("Authorization");
                                 opts.Mask = "******";
                                 opts.ExcludeStaticProperties = true;
                             })
                             .Filter.UniqueOverTimeSpan(@event => @event.Level == LogEventLevel.Error 
                                                                  && @event.HasProperty("EventSourceName", "Azure-Messaging-ServiceBus")
                                                                  && @event.HasProperty("Source", "WX.Messaging.Subscriber.EventHub.EventHubSubscriberRunner"), 
                                 TimeSpan.FromSeconds(60));

            });
        }
    }
}