using FsCheck;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class ArrayArbitrary<T> : Arbitrary<T[]>
    {
        private static int Min = 2;
        private static int Max = 10;

        public static Arbitrary<T[]> Create() => new ArrayArbitrary<T>();

        public override Gen<T[]> Generator => 
            from amount in Gen.Choose(Min, Max)
            from result in Gen.ArrayOf(amount, Arb.Generate<T>())
            select result;

        public static void Initialize(int min, int max) => (Min, Max) = (min, max);
    }
}
