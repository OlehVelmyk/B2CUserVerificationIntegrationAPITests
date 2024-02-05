using System;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services
{
    internal interface ICheckProviderConfigurationParser
    {
        CheckProviderConfiguration Parse(string config, Type configType);
    }

    internal class JsonCheckProviderConfigurationParser : ICheckProviderConfigurationParser
    {
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings();

        public CheckProviderConfiguration Parse(string config, Type configType)
        {
            var baseConfigType = typeof(CheckProviderConfiguration);
            if (!baseConfigType.IsAssignableFrom(configType))
                throw new ArgumentException("Invalid configuration type.", nameof(configType));

            var configuration = !string.IsNullOrWhiteSpace(config)
                ? JsonConvert.DeserializeObject(config, configType, _jsonSettings)
                : Activator.CreateInstance(configType);

            return (CheckProviderConfiguration)configuration;
        }
    }
}