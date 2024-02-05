using FsCheck;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class CharGenerators
    {
        public static Gen<char> Chars() =>
            from c in Arb.Generate<char>()
            select c;

        public static Gen<char> Letters() =>
            from c in Arb.Default.Char().Generator
            where char.IsLetter(c)
            select c;

        public static Gen<char> LettersAndDigits() =>
            from c in Arb.Default.Char().Generator
            where char.IsLetterOrDigit(c)
            select c;

        public static Gen<char> Digits() =>
            from c in Arb.Default.Char().Generator
            where char.IsNumber(c)
            select c;

        public static Gen<char> Space() => Gen.Constant(' ');

        public static Gen<char> Dash() => Gen.Constant('-');
    }
}