using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Services.Providers
{
    internal class StatesDetailsProvider : IStatesDetailsProvider
    {
        private readonly IOptionsProvider _optionsProvider;

        public StatesDetailsProvider(IOptionsProvider optionsProvider)
        {
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        }

        public async Task<bool> IsSupportedAsync(string stateCode, string countryCode)
        {
            if (stateCode == null)
                throw new ArgumentNullException(nameof(stateCode));
            if (countryCode == null)
                throw new ArgumentNullException(nameof(countryCode));

            (stateCode, countryCode) = (NormalizeString(stateCode), NormalizeString(countryCode));
            var option = await _optionsProvider.GetAsync<SupportedStatesOption>();
            if (!option.CountrySupportedStates.TryGetValue(countryCode, out var supportedStates))
            {
                //if we have no exclusion configuration about state we assumes that all states are supported
                return true;
            }

            return supportedStates.Contains(stateCode);
        }

        private static string NormalizeString(string s) => s.ToUpperInvariant();
    }
}