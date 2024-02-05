using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class BlackListCountryArbitrary : Arbitrary<BlackListCountry>
    {
        public static Arbitrary<BlackListCountry> Create => new BlackListCountryArbitrary();

        public override Gen<BlackListCountry> Generator =>
            from countryCode in Alpha3CountryCodeGenerators.BlackList()
            select new BlackListCountry
            {
                CountryCode = countryCode
            };
    }
}
