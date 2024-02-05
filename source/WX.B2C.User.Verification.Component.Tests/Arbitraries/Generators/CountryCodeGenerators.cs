using System;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class CountryCodeGenerators
    {
        public static Gen<string> Supported() => Gen.Elements(CountryCodes.Supported);

        public static Gen<string> Supported(VerificationPolicy policy) =>
            policy switch
            {
                VerificationPolicy.Gb          => Gen.Constant(CountryCodes.Gb),
                VerificationPolicy.Apac        => Apac(),
                VerificationPolicy.Eaa         => Eea(),
                VerificationPolicy.Global      => Global(),
                VerificationPolicy.Row         => RoW(),
                VerificationPolicy.Ru          => Gen.Constant(CountryCodes.Ru),
                VerificationPolicy.Us          => Gen.Constant(CountryCodes.Us),
                VerificationPolicy.Ph          => Gen.Constant(CountryCodes.Ph),
                VerificationPolicy.Unsupported => Unsupported(),
                _                              => throw new ArgumentOutOfRangeException(nameof(policy), policy, null)
            };

        public static Gen<string> Invalid() =>
            from countryCode in StringGenerators.LettersOnly(2, 2)
            where !CountryCodes.Supported.Contains(countryCode.ToUpper())
            select countryCode;

        public static Gen<string> Supported(Func<string, bool> predicate) =>
            Supported().Where(predicate);

        public static Gen<string> Unsupported() => Gen.Elements(CountryCodes.Unsupported);

        public static Gen<string> Eea() => Gen.Elements(CountryCodes.Eea);

        public static Gen<string> Apac() => Gen.Elements(CountryCodes.Apac);

        public static Gen<string> RoW() => Gen.Elements(CountryCodes.RoW);

        public static Gen<string> Global() => Gen.Elements(CountryCodes.Global);
    }
}
