using FsCheck;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators
{
    using static StringGenerators;

    public class XPathGenerator
    {
        public static Gen<string> Random()
        {
            return from part1 in LettersOnly(1, 10)
                   from part2 in LettersOnly(1, 20)
                   select $"{part1}{XPath.Separator}{part2}";
        }
    }
}
