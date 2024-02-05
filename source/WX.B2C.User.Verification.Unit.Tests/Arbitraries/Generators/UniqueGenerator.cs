using System;
using System.Collections.Generic;
using FsCheck;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators
{
    internal class UniqueGenerator
    {
        public static Gen<T[]> Unique<T>(int amount) where T : IEquatable<T> =>
            from result in Gen.ArrayOf(amount, Arb.Generate<T>())
            where new HashSet<T>(result).Count == amount
            select result;
    }
}
