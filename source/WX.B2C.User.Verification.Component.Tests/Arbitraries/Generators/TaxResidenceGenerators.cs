using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class TaxResidenceGenerators
    {
        private const int MinLength = 1;
        private const int MaxLength = 10;

        public static Gen<string[]> Supported() =>
            from length in Gen.Choose(MinLength, MaxLength)
            from countries in Gen.ArrayOf(length, CountryCodeGenerators.Supported())
            select countries;

        public static Gen<string[]> Invalid() =>
            from length in Gen.Choose(MinLength, MaxLength)
            from countries in Gen.ArrayOf(length, CountryCodeGenerators.Invalid())
            select countries;

        public static Gen<string[]> WithDuplicateValues() =>
            from taxResidence in Arb.Generate<TaxResidence>()
            from duplicateValue in Gen.Elements(taxResidence.Countries)
            select taxResidence.Countries.Concat(new[] { duplicateValue }).ToArray();

        public static Gen<string[]> WithNullValues() =>
            from length in Gen.Choose(MinLength, MaxLength)
            from counties in Gen.ArrayOf(length, CountryCodeGenerators.Supported().OrNull())
            where counties.Any(country => country is null)
            select counties;
    }
}
