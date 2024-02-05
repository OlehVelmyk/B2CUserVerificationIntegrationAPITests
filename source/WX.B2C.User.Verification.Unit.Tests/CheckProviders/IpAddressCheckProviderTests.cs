using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Services.System;

namespace WX.B2C.User.Verification.Unit.Tests.CheckProviders
{
    public class IpAddressCheckProviderTests
    {
        private readonly IIpAddressLocationProvider _ipAddressProvider;
        private readonly IpMatchStrategyFactory _ipMatchStrategyFactory;

        public IpAddressCheckProviderTests()
        {
            var matchStrategies = new Dictionary<IpAddressMatchingType, IIpMatchStrategy>
            {
                { IpAddressMatchingType.ByCountry, new IpMatchByCountryStrategy() },
                { IpAddressMatchingType.ByState, new IpMatchByStateStrategy() }
            };
            _ipMatchStrategyFactory = new IpMatchStrategyFactory(matchStrategies);
            _ipAddressProvider = Substitute.For<IIpAddressLocationProvider>();
        }

        [Test]
        public async Task ShouldReturnPassedResult_WhenResolvedCountryMatchResidenceCountry()
        {
            var ipAddress = IPAddress.Parse("2a05:9cc3:7b:b40d:fcd5:bfc4:343:5742");
            var ipAddressLocation = new IpAddressLocation { CountryCode = "UK" };
            var residenceAddress = new AddressDto() { Country = "UK" };

            // Given
            _ipAddressProvider.LookupAsync(Arg.Is(ipAddress))
                              .Returns(Option.Some(ipAddressLocation));

            // Arrange
            var configuration = new IpAddressCheckConfiguration { MatchType = IpAddressMatchingType.ByCountry };

            var checkInputData = new IpAddressCheckData
            {
                IpAddress = ipAddress,
                ResidenceAddress = residenceAddress
            };
            var checkProvider = new IpAddressCheckRunner(_ipAddressProvider, _ipMatchStrategyFactory, configuration);

            // Act & Assert
            var runningContext = await checkProvider.RunAsync(checkInputData);
            runningContext.Should().NotBeNull();

            var processingContext = new CheckProcessingContext(new CheckExternalDataDto());
            var checkResult = await checkProvider.GetResultAsync(processingContext);
            checkResult.Should().NotBeNull();
            checkResult.IsPassed.Should().BeTrue();
        }
        
        [Test]
        public async Task ShouldReturnFailedResult_WhenResolvedCountryDoesNotMatchResidenceCountry()
        {
            var ipAddress = IPAddress.Parse("2a05:9cc3:7b:b40d:fcd5:bfc4:343:5742");
            var ipAddressLocation = new IpAddressLocation { CountryCode = "US" };
            var residenceAddress = new AddressDto() { Country = "UK" };

            // Given
            _ipAddressProvider.LookupAsync(Arg.Is(ipAddress))
                              .Returns(Option.Some(ipAddressLocation));

            // Arrange
            var configuration = new IpAddressCheckConfiguration { MatchType = IpAddressMatchingType.ByCountry };

            var checkInputData = new IpAddressCheckData
            {
                IpAddress = ipAddress,
                ResidenceAddress = residenceAddress
            };
            var checkProvider = new IpAddressCheckRunner(_ipAddressProvider, _ipMatchStrategyFactory, configuration);

            // Act & Assert
            var runningContext = await checkProvider.RunAsync(checkInputData);
            runningContext.Should().NotBeNull();

            var processingContext = new CheckProcessingContext(new CheckExternalDataDto());
            var checkResult = await checkProvider.GetResultAsync(processingContext);
            checkResult.Should().NotBeNull();
            checkResult.IsPassed.Should().BeFalse();
        }

        [Test]
        public async Task ShouldReturnPassedResult_WhenResolvedCountryDoesNotMatchResidenceCountry_AndAlwaysPassedModeSet()
        {
            var ipAddress = IPAddress.Parse("2a05:9cc3:7b:b40d:fcd5:bfc4:343:5742");
            var ipAddressLocation = new IpAddressLocation { CountryCode = "US" };
            var residenceAddress = new AddressDto() { Country = "UK" };

            // Given
            _ipAddressProvider.LookupAsync(Arg.Is(ipAddress))
                              .Returns(Option.Some(ipAddressLocation));

            // Arrange
            var configuration = new IpAddressCheckConfiguration { MatchType = IpAddressMatchingType.ByCountry, ExtractOnly = true };

            var checkInputData = new IpAddressCheckData
            {
                IpAddress = ipAddress,
                ResidenceAddress = residenceAddress
            };
            var checkProvider = new IpAddressCheckRunner(_ipAddressProvider, _ipMatchStrategyFactory, configuration);

            // Act & Assert
            var runningContext = await checkProvider.RunAsync(checkInputData);
            runningContext.Should().NotBeNull();

            var processingContext = new CheckProcessingContext(new CheckExternalDataDto());
            var checkResult = await checkProvider.GetResultAsync(processingContext);
            checkResult.Should().NotBeNull();
            checkResult.IsPassed.Should().BeTrue();
        }

    }
}
