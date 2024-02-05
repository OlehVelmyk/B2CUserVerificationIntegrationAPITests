using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class CharGenerators
    {
        public static Gen<char> Letters() => Arb.Default.Char().Generator.Where(char.IsLetter);

        public static Gen<char> LettersAndDigits() => Arb.Default.Char().Generator.Where(char.IsLetterOrDigit);

        public static Gen<char> Digits() => Arb.Default.Char().Generator.Where(char.IsNumber);
    }
}