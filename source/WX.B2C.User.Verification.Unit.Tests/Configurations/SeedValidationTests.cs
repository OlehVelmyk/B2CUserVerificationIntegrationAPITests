using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.User.Verification.Configuration.Seed;

namespace WX.B2C.User.Verification.Unit.Tests.Configurations
{
    [TestFixture]
    public class SeedValidationTests
    {
        [Test]
        public void ShouldSupportedCountryHaveRegion()
        {
            var countries = SupportedCountries.Seed;
            var regions = Regions.Seed;

            var _ = new AssertionScope();
            foreach (var country in countries)
            {
                var region = regions.FirstOrDefault(region => region.Countries.Contains(country));
                region.Should().NotBeNull($"Regions must be set for {country}");
            }
        }
    }
}