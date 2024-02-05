using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class PositiveDecimalArbitrary : Arbitrary<PositiveDecimal>
    {
        public static Arbitrary<PositiveDecimal> Create() => new PositiveDecimalArbitrary();

        public override Gen<PositiveDecimal> Generator =>
            from value in Arb.Default.Decimal().Generator
            let positiveValue = Math.Abs(value)
            select new PositiveDecimal { Value = positiveValue };
    }
}
