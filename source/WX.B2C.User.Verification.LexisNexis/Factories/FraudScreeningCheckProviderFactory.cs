using System;
using WX.B2C.User.Verification.LexisNexis.Configurations;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Processors;
using WX.B2C.User.Verification.LexisNexis.Runners;
using WX.B2C.User.Verification.LexisNexis.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.LexisNexis.Factories
{
    internal sealed class FraudScreeningCheckProviderFactory : BaseCheckProviderFactory<FraudScreeningCheckConfiguration>
    {
        private readonly IRdpApiClientFactory _clientFactory;
        private readonly ICheckRequestMapper _checkRequestMapper;

        public FraudScreeningCheckProviderFactory(
            IRdpApiClientFactory clientFactory,
            ICheckRequestMapper checkRequestMapper) 
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _checkRequestMapper = checkRequestMapper ?? throw new ArgumentNullException(nameof(checkRequestMapper));
        }

        protected override CheckProvider Create(FraudScreeningCheckConfiguration configuration)
        {
            var checkValidator = new FraudScreeningCheckDataValidator(configuration);
            var checkResultProcessor = new FraudScreeningCheckProcessor();
            var checkRunner = new FraudScreeningCheckRunner(_clientFactory, checkResultProcessor, _checkRequestMapper, configuration);
            return CheckProvider.Create(checkValidator, checkRunner);
        }
    }
}
