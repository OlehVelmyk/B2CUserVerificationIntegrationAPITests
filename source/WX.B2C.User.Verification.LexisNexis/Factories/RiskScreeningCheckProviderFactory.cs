using System;
using WX.B2C.User.Verification.LexisNexis.Configurations;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Processors;
using WX.B2C.User.Verification.LexisNexis.Runners;
using WX.B2C.User.Verification.LexisNexis.Validators;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.LexisNexis.Factories
{
    internal sealed class RiskScreeningCheckProviderFactory : BaseCheckProviderFactory<RiskScreeningCheckConfiguration>
    {
        private readonly IBridgerApiClientFactory _clientFactory;
        private readonly ICheckRequestMapper _checkRequestMapper;

        public RiskScreeningCheckProviderFactory(
            IBridgerApiClientFactory clientFactory,
            ICheckRequestMapper checkRequestMapper)
        {
            _checkRequestMapper = checkRequestMapper ?? throw new ArgumentNullException(nameof(checkRequestMapper));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        protected override CheckProvider Create(RiskScreeningCheckConfiguration configuration)
        {
            var checkValidator = new RiskScreeningCheckDataValidator(configuration);
            var checkResultProcessor = new RiskScreeningCheckProcessor();
            var checkRunner = new RiskScreeningCheckRunner(configuration, _clientFactory, checkResultProcessor, _checkRequestMapper);
            return CheckProvider.Create(checkValidator, checkRunner);
        }
    }
}