using System;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public interface ICheckProviderFactory
    {
        CheckProvider Create(CheckProviderConfiguration configuration);
    }

    public abstract class BaseCheckProviderFactory<TConfig> : ICheckProviderFactory where TConfig : CheckProviderConfiguration, new()
    {
        public CheckProvider Create(CheckProviderConfiguration configuration)
        {
            if (configuration is not TConfig typedConfig)
                throw new ArgumentException("Invalid provider configuration.", nameof(configuration));

            return Create(typedConfig);
        }

        protected abstract CheckProvider Create(TConfig configuration);
    }
}