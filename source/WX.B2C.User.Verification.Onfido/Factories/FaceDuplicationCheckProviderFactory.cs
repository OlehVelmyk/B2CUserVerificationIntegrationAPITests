using System;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Onfido.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Onfido.Factories
{
    internal sealed class FaceDuplicationCheckProviderFactory : BaseCheckProviderFactory<FaceDuplicationCheckConfiguration>
    {
        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly IFaceDuplicationCheckResultProcessor _checkResultProcessor;

        public FaceDuplicationCheckProviderFactory(
            IOnfidoApiClientFactory clientFactory,
            IFaceDuplicationCheckResultProcessor checkResultValidator)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _checkResultProcessor = checkResultValidator ?? throw new ArgumentNullException(nameof(checkResultValidator));
        }

        protected override CheckProvider Create(FaceDuplicationCheckConfiguration configuration)
        {
            var checkInputValidator = new FaceDuplicationCheckDataValidator(configuration);
            var checkRunner = new FaceDuplicationCheckRunner(configuration, _clientFactory, _checkResultProcessor);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}