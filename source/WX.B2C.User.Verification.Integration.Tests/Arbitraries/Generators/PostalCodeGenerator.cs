using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Optional.Collections;
using Optional.Unsafe;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class PostalCodeGenerator
    {
        private static readonly Dictionary<string, string> CountryAndPostcodeTemplate = new()
        {
            ["GB"] = "AAN NAA",
            ["FR"] = "NNNNN",
            ["DE"] = "NNNNN",
            ["SG"] = "NNNNNN",
            ["SE"] = "NNN NN",
            ["ES"] = "NNNNN",
            ["JP"] = "NNN-NNNN",
            ["MX"] = "NNNNN",
            ["NL"] = "NNNN",
            ["IT"] = "NNNNN",
            ["US"] = "NNNNN"
        };

        public static Gen<string> Countries() => Gen.Elements(CountryAndPostcodeTemplate.Keys.ToArray());

        public static Gen<string> PostcodeTemplates() => Gen.Elements(CountryAndPostcodeTemplate.Values.ToArray());

        public static Gen<string> PostcodeByCountry(string country)
        {
            var template = CountryAndPostcodeTemplate.GetValueOrNone(country).ValueOrFailure();
            return PostcodeByTemplate(template);
        }

        public static Gen<string> PostcodeByTemplate(string template) =>
            from postcode in Postcode(template.ToArray())
            select new string(postcode, 0, postcode.Length);

        private static Gen<char[]> Postcode(char[] template)
        {
            var gen = template.Select(c => c switch
            {
                'A' => CharGenerators.Letters(),
                'N' => CharGenerators.Digits(),
                ' ' => CharGenerators.Space(),
                '-' => CharGenerators.Dash(),
                _   => throw new ArgumentOutOfRangeException(nameof(c), "Invalid symbol in template.")
            }).ToArray();

            return Gen.Sequence(gen);
        }
    }
}
