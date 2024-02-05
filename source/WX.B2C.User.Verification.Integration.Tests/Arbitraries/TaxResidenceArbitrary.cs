using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class TaxResidenceArbitrary : Arbitrary<TaxResidence>
    {
        public static Arbitrary<TaxResidence> Create() => new TaxResidenceArbitrary();

        public override Gen<TaxResidence> Generator =>
            from length in Gen.Choose(1, 5)
            from countries in Gen.ArrayOf(length, CountryCodeGenerators.Countries())
            from hasValue in Arb.Generate<bool>()
            select new TaxResidence(hasValue, countries);
    }

    internal class NotEmptyTaxResidenceArbitrary : Arbitrary<TaxResidence>
    {
        public static Arbitrary<TaxResidence> Create() => new NotEmptyTaxResidenceArbitrary();

        public override Gen<TaxResidence> Generator =>
            from length in Gen.Choose(1, 5)
            from countries in Gen.ArrayOf(length, CountryCodeGenerators.Countries())
            select new TaxResidence(true, countries);
    }
}