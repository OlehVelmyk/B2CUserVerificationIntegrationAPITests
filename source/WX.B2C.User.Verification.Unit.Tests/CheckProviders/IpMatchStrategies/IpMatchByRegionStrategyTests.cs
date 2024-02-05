using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Services.System;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.CheckProviders.IpMatchStrategies
{
    internal class IpMatchByRegionStrategyTests
    {
        private readonly IpMatchByRegionStrategy _sut;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public IpMatchByRegionStrategyTests()
        {
            _countryDetailsProvider = Substitute.For<ICountryDetailsProvider>();
            _sut = new IpMatchByRegionStrategy(_countryDetailsProvider);

            Arb.Register<SupportedCountryArbitrary>();
            Arb.Register<TwoDifferentArbitrary<SupportedCountry>>();
        }

        [Theory(MaxTest = 10)]
        public async Task ShouldMatch(AddressDto residenceAddress,
                                      IpAddressLocation ipAddressLocation)
        {
            var expectedRegion = ipAddressLocation.ContinentCode;

            // Given
            _countryDetailsProvider.GetRegionAsync(Arg.Is(residenceAddress.Country)).Returns(expectedRegion);
            _countryDetailsProvider.GetRegionAsync(Arg.Is(ipAddressLocation.CountryCode)).Returns(expectedRegion);

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

            _countryDetailsProvider.GetRegionAsync(Arg.Is(residenceAddress.Country)).Returns("TEST_REGION");
            _countryDetailsProvider.GetRegionAsync(Arg.Is(ipAddressLocation.CountryCode)).Returns(ipAddressLocation.ContinentCode);

            // Act
            var result = await _sut.MatchAsync(ipAddressLocation, residenceAddress);

            // Assert
            result.Should().BeFalse();
        }
    }
}