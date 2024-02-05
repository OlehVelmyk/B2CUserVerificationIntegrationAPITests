using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class FeatureToggleService : IFeatureToggleService
    {
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IStatesDetailsProvider _statesDetailsProvider;

        public FeatureToggleService(
            ICountryDetailsProvider countryDetailsProvider,
            IStatesDetailsProvider statesDetailsProvider)
        {
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _statesDetailsProvider = statesDetailsProvider ?? throw new ArgumentNullException(nameof(statesDetailsProvider));
        }

        public async Task<bool> IsVerificationAvailableAsync(VerificationPolicySelectionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var isSupported = await _countryDetailsProvider.IsSupportedAsync(context.Country);
            if (isSupported && context.State != null)
                isSupported = await _statesDetailsProvider.IsSupportedAsync(context.State, context.Country);

            return isSupported;
        }
    }
}