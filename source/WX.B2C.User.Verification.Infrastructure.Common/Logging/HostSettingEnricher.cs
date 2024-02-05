using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Logging
{
    internal class HostSettingEnricher : ILogEventEnricher
    {
        private readonly Dictionary<string, string> _properties;

        public HostSettingEnricher(IHostSettingsProvider hostSettingProvider)
        {
            _properties = new Dictionary<string, string>()
            {
                {HostSettingsKey.HostName, hostSettingProvider.GetSetting(HostSettingsKey.HostName)},
                {HostSettingsKey.NodeName, hostSettingProvider.GetSetting(HostSettingsKey.NodeName)},
            };
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
        {
            foreach (var eventProperty in _properties)
            {
                logEvent.AddOrUpdateProperty(factory.CreateProperty(eventProperty.Key,eventProperty.Value));
            }
        }
    }
}