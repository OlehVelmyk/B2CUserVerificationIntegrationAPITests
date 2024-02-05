using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Services.System;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.CheckProviders.IpMatchStrategies
{
    internal class IpMatchByStateStrategyTests
    {
        private readonly IpMatchByStateStrategy _sut;

        public IpMatchByStateStrategyTests()
        {
            _sut = new IpMatchByStateStrategy();

            Arb.Register<SupportedCountryArbitrary>();
            Arb.Register<TwoDifferentArbitrary<SupportedCountry>>();
        }

        [Theory(MaxTest = 10)]
        public async Task ShouldMatch(AddressDto residenceAddress,
                                      IpAddressLocation ipAddressLocation)
        {
            // Arrange
            ipAddressLocation.CountryCode = residenceAddress.Country;
            ipAddressLocation.StateCode = residenceAddress.State;

            // Act
            var result = await _sut.MatchAsync(ipAddressLocation, residenceAddress);

            // Assert
            result.Should().BeTrue();
        }

        [Theory(MaxTest = 10)]
        public async Task ShouldNotMatch(AddressDto residenceAddress,
                                         IpAddressLocation ipAddressLocation,
                                         TwoDifferent<SupportedCountry> twoCountries)
        {
            // Given
            (residenceAddress.Country, ipAddressLocation.CountryCode) = twoCountries;

            // Act
            var result = await _sut.MatchAsync(ipAddressLocation, residenceAddress);

            // Assert
            result.Should().BeFalse();
        }
    }
}