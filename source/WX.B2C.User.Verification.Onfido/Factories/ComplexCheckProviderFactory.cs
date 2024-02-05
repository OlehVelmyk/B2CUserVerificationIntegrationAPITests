using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Onfido.Validators;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Factories
{
    internal class ComplexCheckProviderFactory : BaseCheckProviderFactory<ComplexCheckConfiguration>
    {
        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public ComplexCheckProviderFactory(IOnfidoApiClientFactory clientFactory, ICountryDetailsProvider countryDetailsProvider)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        protected override CheckProvider Create(ComplexCheckConfiguration configuration)
        {
            var checkInputValidator = new ComplexCheckDataValidator(configuration);
            var checkRunner = new ComplexCheckRunner(configuration, _clientFactory, _countryDetailsProvider);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}
