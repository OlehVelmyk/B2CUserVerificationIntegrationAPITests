using FsCheck;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators
{
    public static class CharGenerators
    {
        public static Gen<char> Chars()
        {
            return from c in Arb.Generate<char>()
                   select c;
        }

        public static Gen<char> Letters()
        {
            return from c in Arb.Default.Char().Generator
                   where char.IsLetter(c)
                   select c;
        }

        public static Gen<char> LettersAndDigits()
        {
            return from c in Arb.Default.Char().Generator
                   where char.IsLetterOrDigit(c)
                   select c;
        }

        public static Gen<char> Digits()
        {
            return from c in Arb.Default.Char().Generator
                   where char.IsNumber(c)
                   select c;
        }
    }
}