using System.Linq;
using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class ArrayGenerator
    {
        public static Gen<T[]> Unique<T>(int amount) =>
            from array in Gen.ArrayOf(amount * 2, Arb.Generate<T>())
            let result = array.Distinct().Take(amount).ToArray()
            where result.Length == amount
            select result;
    }
}
