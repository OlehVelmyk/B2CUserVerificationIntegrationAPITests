using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IpAddressCheckProviderFactory : BaseCheckProviderFactory<IpAddressCheckConfiguration>
    {
        private readonly IIpAddressLocationProvider _ipAddressProvider;
        private readonly IpMatchStrategyFactory _ipMatchStrategyFactory;

        public IpAddressCheckProviderFactory(
            IIpAddressLocationProvider ipAddressProvider,
            IpMatchStrategyFactory ipMatchStrategyFactory)
        {
            _ipAddressProvider = ipAddressProvider ?? throw new ArgumentNullException(nameof(ipAddressProvider));
            _ipMatchStrategyFactory = ipMatchStrategyFactory ?? throw new ArgumentNullException(nameof(ipMatchStrategyFactory));
        }

        protected override CheckProvider Create(IpAddressCheckConfiguration configuration)
        {
            var checkDataValidator = new IpAddressCheckDataValidator(configuration);
            var checkRunner = new IpAddressCheckRunner(_ipAddressProvider, _ipMatchStrategyFactory, configuration);
            return CheckProvider.Create(checkDataValidator, checkRunner);
        }
    }
}