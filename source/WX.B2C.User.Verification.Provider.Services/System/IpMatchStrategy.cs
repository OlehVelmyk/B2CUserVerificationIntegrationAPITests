using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    public interface IIpMatchStrategy
    {
        /// <summary>
        /// Matches resolved by IP location with residence address provided by user.
        /// </summary>
        /// <param name="ipAddressLocation">IP location details.</param>
        /// <param name="userAddress">Residence address of user.</param>
        /// <returns>Result of matching.</returns>
        public Task<bool> MatchAsync(IpAddressLocation ipAddressLocation, AddressDto userAddress);
    }

    public sealed class IpMatchByStateStrategy : IIpMatchStrategy
    {
        public Task<bool> MatchAsync(IpAddressLocation ipAddressLocation, AddressDto userAddress)
        {
            if (ipAddressLocation == null)
                throw new ArgumentNullException(nameof(ipAddressLocation));

            if (userAddress == null)
                throw new ArgumentNullException(nameof(userAddress));

            var ipLocationCountryCode = ipAddressLocation.CountryCode?.ToUpperInvariant();
            var ipLocationStateCode = ipAddressLocation.StateCode?.ToUpperInvariant();

            var residenceCountryCode = userAddress.Country?.ToUpperInvariant();
            var residenceStateCode = userAddress.State?.ToUpperInvariant();

            return Task.FromResult(ipLocationCountryCode == residenceCountryCode
                                && ipLocationStateCode == residenceStateCode);
        }
    }

    public sealed class IpMatchByCountryStrategy : IIpMatchStrategy
    {
        public Task<bool> MatchAsync(IpAddressLocation ipAddressLocation, AddressDto userAddress)
        {
            if (ipAddressLocation == null)
                throw new ArgumentNullException(nameof(ipAddressLocation));

            if (userAddress == null)
                throw new ArgumentNullException(nameof(userAddress));

            var ipLocationCountryCode = ipAddressLocation.CountryCode?.ToUpperInvariant();
            var residenceCountryCode = userAddress.Country?.ToUpperInvariant();

            return Task.FromResult(ipLocationCountryCode == residenceCountryCode);
        }
    }

    public sealed class IpMatchByRegionStrategy : IIpMatchStrategy
    {
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public IpMatchByRegionStrategy(ICountryDetailsProvider countryDetailsProvider)
        {
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        public async Task<bool> MatchAsync(IpAddressLocation ipAddressLocation, AddressDto userAddress)
        {
            if (ipAddressLocation == null)
                throw new ArgumentNullException(nameof(ipAddressLocation));

            if (userAddress == null)
                throw new ArgumentNullException(nameof(userAddress));

            var ipLocationRegion = await _countryDetailsProvider.GetRegionAsync(ipAddressLocation.CountryCode);
            var residenceRegion = await _countryDetailsProvider.GetRegionAsync(userAddress.Country);

            return ipLocationRegion == residenceRegion;
        }
    }
}