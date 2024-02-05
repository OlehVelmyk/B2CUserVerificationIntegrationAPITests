using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Services.Configuration
{
    public class OptionsProvider : IOptionsProvider
    {
        private readonly IEnumerable<IOptionProvider> _providers;

        public OptionsProvider(IEnumerable<IOptionProvider> providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        public Task<T> GetAsync<T>() where T : Option
        {
            var provider = _providers.OfType<IOptionProvider<T>>().FirstOrDefault();
            if (provider == null)
                throw new InvalidOperationException($"Cannot find option provider for type {typeof(T).Name}");

            return provider.GetAsync();
        }
    }
}