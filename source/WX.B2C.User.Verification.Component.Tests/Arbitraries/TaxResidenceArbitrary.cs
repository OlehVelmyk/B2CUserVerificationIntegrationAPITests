using System;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class TaxResidenceArbitrary : Arbitrary<TaxResidence>
    {
        public static Arbitrary<TaxResidence> Create() => new TaxResidenceArbitrary();

        public override Gen<TaxResidence> Generator =>
            from countries in TaxResidenceGenerators.Supported()
            select new TaxResidence { Countries = countries.Distinct().ToArray() };
    }

    internal class InvalidTaxResidenceArbitrary : Arbitrary<InvalidTaxResidence>
    {
        public static Arbitrary<InvalidTaxResidence> Create() => new InvalidTaxResidenceArbitrary();

        public override Gen<InvalidTaxResidence> Generator =>
            from countries in Gen.OneOf(TaxResidenceGenerators.Invalid(),
                                        TaxResidenceGenerators.WithDuplicateValues(),
                                        TaxResidenceGenerators.WithNullValues(),
                                        Gen.Constant(Array.Empty<string>()))
            select new InvalidTaxResidence { Countries = countries };
    }
}
