using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class SupportedCountryArbitrary : Arbitrary<SupportedCountry>
    {
        public static Arbitrary<SupportedCountry> Create() => new SupportedCountryArbitrary();

        public override Gen<SupportedCountry> Generator =>
            from countryCode in CountryCodeGenerators.Supported()
            select new SupportedCountry(countryCode);
    }
}