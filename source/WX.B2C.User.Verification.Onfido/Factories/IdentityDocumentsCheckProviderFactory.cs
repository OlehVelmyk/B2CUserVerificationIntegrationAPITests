using System;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Onfido.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Onfido.Factories
{
    internal sealed class IdentityDocumentsCheckProviderFactory : BaseCheckProviderFactory<IdentityDocumentsCheckConfiguration>
    {
        private readonly IIdentityDocumentCheckResultProcessor _checkResultProcessor;
        private readonly IOnfidoApiClientFactory _clientFactory;

        public IdentityDocumentsCheckProviderFactory(
            IOnfidoApiClientFactory clientFactory,
            IIdentityDocumentCheckResultProcessor checkResultProcessor)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override CheckProvider Create(IdentityDocumentsCheckConfiguration configuration)
        {
            var checkInputValidator = new IdentityCheckDataValidator(configuration);
            var checkRunner = new IdentityDocumentsCheckRunner(configuration, _clientFactory, _checkResultProcessor);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}