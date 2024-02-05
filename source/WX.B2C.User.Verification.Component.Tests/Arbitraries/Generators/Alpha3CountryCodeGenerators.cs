using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Constants;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class Alpha3CountryCodeGenerators
    {
        public static Gen<string> BlackList() => Gen.Elements(Alpha3CountryCodes.BlackList);
    }
}
