using FsCheck;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    internal class SupportedCountryArbitrary : Arbitrary<SupportedCountry>
    {
        public static Arbitrary<SupportedCountry> Create() => new SupportedCountryArbitrary();
        
        public override Gen<SupportedCountry> Generator => 
            from country in CountryCodeGenerators.Countries()
            select new SupportedCountry { Value = country};
    }
}
