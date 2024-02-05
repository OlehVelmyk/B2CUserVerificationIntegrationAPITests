using System;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Onfido.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Onfido.Factories
{
    internal sealed class FacialSimilarityCheckProviderFactory : BaseCheckProviderFactory<FacialSimilarityCheckConfiguration>
    {
        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly IFacialSimilarityCheckResultProcessor _checkResultProcessor;

        public FacialSimilarityCheckProviderFactory(
            IOnfidoApiClientFactory clientFactory,
            IFacialSimilarityCheckResultProcessor checkResultProcessor)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _checkResultProcessor = checkResultProcessor ?? throw new ArgumentNullException(nameof(checkResultProcessor));
        }

        protected override CheckProvider Create(FacialSimilarityCheckConfiguration configuration)
        {
            var checkInputValidator = new FacialSimilarityCheckDataValidator(configuration);
            var checkRunner = new FacialSimilarityCheckRunner(configuration, _clientFactory, _checkResultProcessor);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}