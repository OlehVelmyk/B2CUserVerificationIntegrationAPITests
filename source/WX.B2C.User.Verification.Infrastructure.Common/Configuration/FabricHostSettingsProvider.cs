using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Configuration
{
    public class FabricHostSettingsProvider : IHostSettingsProvider
    {
        private readonly ICodePackageActivationContext _activationContext;

        public FabricHostSettingsProvider(ICodePackageActivationContext activationContext)
        {
            _activationContext = activationContext ?? throw new ArgumentNullException(nameof(activationContext));
        }

        public string GetSetting(string name)
        {
            if (!TryResolveFromEnvironment(name, out var value))
            {
                var properties = GetFromFabricRuntime();
                value = properties.Where(x => x.Name == name)
                                  .Select(x => x.Value)
                                  .FirstOrDefault();
            }

            if (value == null)
                throw new ArgumentNullException($"Parameter {name} is not found in fabric configuration");

            return value;
        }

        private IEnumerable<ConfigurationProperty> GetFromFabricRuntime()
        {
            var targetSections = new[] { "AppConfigSection", "KeyVaultConfigSection" };

            var fabricConfig = _activationContext.GetConfigurationPackageObject("Config");

            return fabricConfig.Settings.Sections
                               .Where(x => targetSections.Contains(x.Name))
                               .SelectMany(x => x.Parameters);
        }

        private bool TryResolveFromEnvironment(string key, out string value)
        {
            value = null;
            switch (key)
            {
                case HostSettingsKey.AppName:
                    value = Environment.GetEnvironmentVariable("Fabric_ApplicationName");
                    return true;
                case HostSettingsKey.ApplicationHost:
                    value = Environment.GetEnvironmentVariable("Fabric_ServicePackageName")?.Replace("Pkg", "");
                    return true;
                case HostSettingsKey.ApplicationPort:
                    value = Environment.GetEnvironmentVariable("Fabric_Endpoint_ServiceEndpoint") ?? 0.ToString();
                    return true;
                case HostSettingsKey.HostName:
                    value = Environment.MachineName;
                    return true;
                case HostSettingsKey.MetricsPort:
                    value = Environment.GetEnvironmentVariable("Fabric_Endpoint_MetricsEndpoint") ?? 0.ToString();
                    return true;
                case HostSettingsKey.NodeName:
                    value = Environment.GetEnvironmentVariable("Fabric_NodeName");
                    return true;
                case HostSettingsKey.Version:
                    value = _activationContext.CodePackageVersion;
                    return true;
                case HostSettingsKey.Environment:
                    value = Environment.GetEnvironmentVariable("Environment");
                    return true;
                case HostSettingsKey.LogLevel:
                    value = Environment.GetEnvironmentVariable("LogLevel");
                    return true;
                default:
                    return false;
            }
        }
    }
}