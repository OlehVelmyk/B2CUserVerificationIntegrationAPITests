using System;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Configuration;
using WX.B2C.User.Verification.PassFort.Mappers;
using WX.B2C.User.Verification.PassFort.Processors;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.PassFort.Runners;
using WX.B2C.User.Verification.PassFort.Validators;

namespace WX.B2C.User.Verification.PassFort.Factories
{
    internal sealed class RiskScreeningCheckProviderFactory : BaseCheckProviderFactory<RiskScreeningCheckConfiguration>
    {
        private readonly IPassFortApiClientFactory _clientFactory;
        private readonly IPassFortProfileUpdater _profileUpdater;
        private readonly IProfileDataPatchMapper _profileDataPatchMapper;

        public RiskScreeningCheckProviderFactory(
            IPassFortApiClientFactory clientFactory,
            IPassFortProfileUpdater profileUpdater,
            IProfileDataPatchMapper profileDataPatchMapper)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _profileUpdater = profileUpdater ?? throw new ArgumentNullException(nameof(profileUpdater));
            _profileDataPatchMapper = profileDataPatchMapper ?? throw new ArgumentNullException(nameof(profileDataPatchMapper));
        }

        protected override CheckProvider Create(RiskScreeningCheckConfiguration configuration)
        {
            var checkInputValidator = new RiskScreeningCheckDataValidator(configuration);
            var checkProcessor = new PassFortCheckProcessor();
            var checkRunner = new RiskScreeningCheckRunner(configuration, _clientFactory, checkProcessor, _profileUpdater, _profileDataPatchMapper);
            return CheckProvider.Create(checkInputValidator, checkRunner);
        }
    }
}
