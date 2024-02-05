using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Optional.Unsafe;
using Serilog.Core;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.IpStack;
using WX.B2C.User.Verification.IpStack.Mappers;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    public class IpStackGatewayTests
    {
        private readonly IpStackGateway _sut;

        public IpStackGatewayTests()
        {
            var appConfig = new AppLocalConfig();

            var logger = Logger.None;
            var clientFactory = new IpStackApiClientFactory(appConfig, logger);
            var mapper = new IpAddressLocationMapper();
            _sut = new IpStackGateway(clientFactory, mapper, logger);
        }

        [Test]
        [Parallelizable]
        [TestCaseSource(nameof(TestIpAddresses))]
        public async Task ShouldReturnValidIpAddressLocation(string ipString)
        {
            // Arrange
            var ipAddress = IPAddress.Parse(ipString);

            // Act
            var result = await _sut.LookupAsync(ipAddress);

            // Assert
            var actualResult = result.ValueOrDefault();
            var expectedResult = ExpectedResults[ipString];

            actualResult.Should().NotBeNull();

            // Exclude latitude and longitude from comparison because it is not deterministic
            // and may change over time for the same IP address. 
            actualResult.Should()
                        .BeEquivalentTo(expectedResult,
                            opt => opt
                                   .Excluding(p => p.Latitude)
                                   .Excluding(p => p.Longitude));
        }

        private static IEnumerable<string> TestIpAddresses => ExpectedResults.Keys;

        private static readonly IDictionary<string, IpAddressLocation> ExpectedResults =
            new Dictionary<string, IpAddressLocation>
            {
                {
                    "95.67.53.46", new IpAddressLocation
                    {
                        ContinentCode = "EU",
                        ContinentName = "Europe",
                        CountryCode = "UA",
                        CountryName = "Ukraine",
                        StateCode = "30",
                        StateName = "Kyiv City",
                        City = "Kyiv",
                        Zip = "01000",
                        Latitude = 50.43333053588867,
                        Longitude = 30.51667022705078
                    }
                },
                {
                    "52.232.126.99", new IpAddressLocation
                    {
                        ContinentCode = "EU",
                        ContinentName = "Europe",
                        CountryCode = "NL",
                        CountryName = "Netherlands",
                        StateCode = "NH",
                        StateName = "North Holland",
                        City = "Diemen",
                        Zip = "1101",
                        Latitude = 52.309051513671875,
                        Longitude = 4.940189838409424
                    }
                },
                {
                    "2001:288:302a::192:192:22:1", new IpAddressLocation
                    {
                        ContinentCode = "AS",
                        ContinentName = "Asia",
                        CountryCode = "TW",
                        CountryName = "Taiwan",
                        StateCode = "CYI",
                        StateName = "Taiwan",
                        City = "Taoyuan City",
                        Zip = "330",
                        Latitude = 24.98889923095703,
                        Longitude = 121.31109619140625
                    }
                },
                {
                    "2002:5be4:34e2:0:0:0:0:0", new IpAddressLocation
                    {
                        ContinentCode = "NA",
                        ContinentName = "North America",
                        CountryCode = "US",
                        CountryName = "United States",
                        StateCode = "CA",
                        StateName = "California",
                        City = "Santa Monica",
                        Zip = "90292",
                        Latitude = 33.979000091552734,
                        Longitude = -118.4530029296875
                    }
                }
            };
    }
}
