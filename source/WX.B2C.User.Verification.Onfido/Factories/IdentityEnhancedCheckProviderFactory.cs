using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Onfido.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Onfido.Factories
{
    internal class IdentityEnhancedCheckProviderFactory : BaseCheckProviderFactory<IdentityEnhancedCheckConfiguration>
    {
        private readonly IIdentityEnhancedCheckResultProcessor _checkResultProcessor;
        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public IdentityEnhancedCheckProviderFactory(
            IOnfidoApiClientFactory clientFactory,
            IIdentityEnhancedCheckResultProcessor checkResultProcessor,
            ICountryDetailsProvider countryDetailsProvider)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override CheckProvider Create(IdentityEnhancedCheckConfiguration configuration)
        {
            var checkInputValidator = new IdentityEnhancedCheckDataValidator(configuration);
            var checkRunner = new IdentityEnhancedCheckRunner(configuration, _clientFactory, _checkResultProcessor, _countryDetailsProvider);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}