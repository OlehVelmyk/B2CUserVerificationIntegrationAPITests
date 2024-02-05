using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    public enum IpAddressMatchingType
    {
        ByCountry = 1,
        ByState = 2,
        ByRegion = 3
    }

    public sealed class IpMatchStrategyFactory
    {
        private readonly IDictionary<IpAddressMatchingType, IIpMatchStrategy> _supportedStrategies;

        public IpMatchStrategyFactory(IDictionary<IpAddressMatchingType, IIpMatchStrategy> supportedStrategies)
        {
            _supportedStrategies = supportedStrategies ?? throw new ArgumentNullException(nameof(supportedStrategies));
        }

        public IIpMatchStrategy Create(IpAddressMatchingType matchType)
        {
            if (!_supportedStrategies.ContainsKey(matchType))
                throw new CheckExecutionException(ErrorCodes.UnsupportedFunctionality, "Unsupported IP match type.");

            return _supportedStrategies[matchType];
        }
    }
}