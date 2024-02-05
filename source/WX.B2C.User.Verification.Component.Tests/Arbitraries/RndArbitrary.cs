using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class RndArbitrary : Arbitrary<Rnd>
    {
        private const int MinValue = 1000000;
        private Gen<ulong> Ulong =>
            Gen.Choose(MinValue, int.MaxValue).Select(n => (ulong)n);

        public static Arbitrary<Rnd> Create() => new RndArbitrary();

        public override Gen<Rnd> Generator =>
            from seed in Ulong
            from gamma in Ulong
            where gamma % 2 == 1
            select new Rnd(seed, gamma);
    }
}
