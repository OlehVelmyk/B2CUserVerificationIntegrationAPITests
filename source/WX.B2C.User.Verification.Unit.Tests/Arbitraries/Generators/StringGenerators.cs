using FsCheck;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators
{
    public static class StringGenerators
    {
        public static Gen<string> NotEmpty(int maxLength)
        {
            return NotEmpty(1, maxLength);
        }

        public static Gen<string> NotEmpty(int minLength, int maxLength)
        {
            return from length in Gen.Choose(minLength, maxLength)
                   from chars in Gen.ArrayOf(length, CharGenerators.LettersAndDigits())
                   select new string(chars, 0, length);
        }

        public static Gen<string> FileName(int maxLength)
        {
            return from length in Gen.Choose(1, maxLength)
                   from chars in Gen.ArrayOf(length, CharGenerators.LettersAndDigits())
                   select new string(chars, 0, length);
        }

        public static Gen<string> LettersOnly(int minLength, int maxLength)
        {
            return from length in Gen.Choose(minLength, maxLength)
                   from chars in Gen.ArrayOf(length, CharGenerators.Letters())
                   select new string(chars, 0, length);
        }

        public static Gen<string> Numbers(int maxLength)
        {
            return from chars in Gen.ArrayOf(maxLength, CharGenerators.Digits())
                   select new string(chars);
        }
    }
}