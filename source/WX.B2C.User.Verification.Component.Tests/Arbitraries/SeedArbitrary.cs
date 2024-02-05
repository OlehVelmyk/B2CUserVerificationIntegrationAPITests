using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class SeedArbitrary : Arbitrary<Seed>
    {
        public static Arbitrary<Seed> Create() => new SeedArbitrary();

        public override Gen<Seed> Generator => 
            from seed in Gen.Choose(int.MinValue, int.MaxValue)
            select new Seed { Value = seed };
    }
}
