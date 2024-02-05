using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.Configuration.Contracts;

namespace WX.B2C.User.Verification.Integration.Tests
{
    internal class SelfHostingValuesResolver : IHostingSpecificValuesResolver
    {
        private readonly string _configurationSection;
        private readonly string _keyVaultSection;
        private readonly string _applicationVersion;

        private readonly IConfigurationRoot _configurationRoot;

        public SelfHostingValuesResolver(string configurationSection, string keyVaultSection)
        {
            _configurationSection = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));
            _keyVaultSection = keyVaultSection ?? throw new ArgumentNullException(nameof(keyVaultSection));
            _configurationRoot = new ConfigurationBuilder().AddJsonFile(GetConfigFilePath()).Build();
            _applicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        public string GetValue(string key)
        {
            if (TryResolveFromEnvironment(key, out var value))
                return value;
            
            if (TryResolveFromKeyVaultSection(key, out value))
                return value;

            return GetSettingsSections(_configurationSection)[key];
        }

        private bool TryResolveFromKeyVaultSection(string key, out string value)
        {
            value = null;
            switch (key)
            {
                case "KeyVaultUrl":
                case "KeyVaultClientId":
                case "KeyVaultSecret":
                    value = GetSettingsSections(_keyVaultSection)[key];
                    return true;
                default:
                    return false;
            }
        }

        private bool TryResolveFromEnvironment(string key, out string value)
        {
            value = null;
            switch (key)
            {
                case nameof(HostSettingsKey.AppName):
                    value = GetSettingsSections(_configurationSection)[key];
                    return true;
                case nameof(HostSettingsKey.Version):
                    value = _applicationVersion;
                    return true;
                case nameof(HostSettingsKey.NodeName):
                    value = Environment.MachineName;
                    return true;
                case nameof(HostSettingsKey.HostName):
                    value = Environment.MachineName;
                    return true;
                case nameof(HostSettingsKey.ApplicationHost):
                    value = GetSettingsSections(_configurationSection)[key];
                    return true;
                case nameof(HostSettingsKey.MetricsPort):
                case nameof(HostSettingsKey.ApplicationPort):
                    value = GetSettingsSections(_configurationSection)[key];
                    return true;
                default:
                    return false;
            }
        }

        private IConfigurationSection GetSettingsSections(string sectionName) => _configurationRoot.GetSection(sectionName);

        private static string GetConfigFilePath()
        {
            var assemblyDirectory = Directory.GetCurrentDirectory();
            var configFilePath = Path.Combine(assemblyDirectory, "appsettings.json");
            if (!File.Exists(configFilePath))
            {
                assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly()
                                                                  .Location);
                configFilePath = Path.Combine(assemblyDirectory, "appsettings.json");
            }

            var configFileData = File.Exists(configFilePath) ? File.ReadAllText(configFilePath) : string.Empty;

            if (string.IsNullOrEmpty(configFileData)) throw new InvalidOperationException("Configuration file not found");

            return configFilePath;
        }
    }

    /// <summary>
    /// TODO Maybe better to remove such behavior + SelfHostingValuesResolver
    /// </summary>
    internal interface IHostingSpecificValuesResolver
    {
        string GetValue(string key);
    }
}