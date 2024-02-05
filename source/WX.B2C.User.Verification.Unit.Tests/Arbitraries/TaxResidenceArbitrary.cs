using FsCheck;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class TaxResidenceArbitrary : Arbitrary<TaxResidence>
    {
        public static Arbitrary<TaxResidence> Create()
        {
            return new TaxResidenceArbitrary();
        }

        public override Gen<TaxResidence> Generator =>
            from length in Gen.Choose(1, 5)
            from countries in Gen.ArrayOf(length, CountryCodeGenerators.Countries())
            from hasValue in Arb.Generate<bool>()
            select new TaxResidence(hasValue, countries);
    }
}