using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IpAddressCheckRunner : SyncCheckRunner<IpAddressCheckData>
    {
        private readonly IpAddressCheckConfiguration _configuration;
        private readonly IIpAddressLocationProvider _ipAddressProvider;
        private readonly IpMatchStrategyFactory _ipMatchStrategyFactory;

        public IpAddressCheckRunner(
            IIpAddressLocationProvider ipAddressProvider,
            IpMatchStrategyFactory ipMatchStrategyFactory,
            IpAddressCheckConfiguration configuration)
        {
            _ipMatchStrategyFactory = ipMatchStrategyFactory ?? throw new ArgumentNullException(nameof(ipMatchStrategyFactory));
            _ipAddressProvider = ipAddressProvider ?? throw new ArgumentNullException(nameof(ipAddressProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task<CheckProcessingResult> RunSync(IpAddressCheckData checkData)
        {
            if (!Enum.IsDefined(typeof(IpAddressMatchingType), _configuration.MatchType))
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "IP match type is not configured for check.");

            var ipMatchStrategy = _ipMatchStrategyFactory.Create(_configuration.MatchType);
            var lookupResult = await _ipAddressProvider.LookupAsync(checkData.IpAddress);
            return await lookupResult.Match(async resolvedLocation =>
            {
                var isMatched = await ipMatchStrategy.MatchAsync(resolvedLocation, checkData.ResidenceAddress);
                var outputData = new IpAddressCheckOutputData { IsIpMatched = isMatched, ResolvedLocation = resolvedLocation };

                return isMatched || _configuration.ExtractOnly
                    ? CheckProcessingResult.Passed(outputData)
                    : CheckProcessingResult.Failed(outputData);
            }, () => throw new CheckExecutionException(ErrorCodes.ProviderUnknownError, "IP address lookup failed."));
        }
    }
}
