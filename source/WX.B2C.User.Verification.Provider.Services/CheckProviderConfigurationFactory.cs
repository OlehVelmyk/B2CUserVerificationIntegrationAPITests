using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Provider.Contracts.IoC;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services
{
    public interface ICheckProviderConfigurationFactory
    {
        CheckProviderConfiguration Get(CheckType checkType, CheckProviderType providerType, string config);

        ComplexCheckConfiguration Get(CheckProviderType providerType, CheckProviderConfiguration[] configurations);
    }

    internal class CheckProviderConfigurationFactory : ICheckProviderConfigurationFactory
    {
        private readonly IDictionary<CheckProviderMetadata, Type> _configurationTypes;
        private readonly ICheckProviderConfigurationParser _configurationParser;

        public CheckProviderConfigurationFactory(
            IDictionary<CheckProviderMetadata, Type> configurationTypes,
            ICheckProviderConfigurationParser configurationParser)
        {
            _configurationTypes = configurationTypes ?? throw new ArgumentNullException(nameof(configurationTypes));
            _configurationParser = configurationParser ?? throw new ArgumentNullException(nameof(configurationParser));
        }

        public CheckProviderConfiguration Get(CheckType checkType, CheckProviderType providerType, string config)
        {
            var configType = GetConfigurationType(checkType, providerType);
            return _configurationParser.Parse(config, configType);
        }

        public ComplexCheckConfiguration Get(CheckProviderType providerType, CheckProviderConfiguration[] configurations)
        {
            return new ComplexCheckConfiguration(configurations);
        }

        private Type GetConfigurationType(CheckType checkType, CheckProviderType providerType)
        {
            var metadata = new CheckProviderMetadata(checkType, providerType);
            
            if (!_configurationTypes.TryGetValue(metadata, out var configType))
                throw new ArgumentOutOfRangeException(nameof(configType), configType, "Unsupported check configuration type.");
            
            return configType;
        }
    }
}
