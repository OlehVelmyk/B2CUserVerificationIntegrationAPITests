using System;
using FsCheck;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    internal class TwoDifferentArbitrary<T> : Arbitrary<TwoDifferent<T>> where T : IEquatable<T>
    {
        public static Arbitrary<TwoDifferent<T>> Create()
        {
            return new TwoDifferentArbitrary<T>();
        }

        public override Gen<TwoDifferent<T>> Generator =>
            from items in UniqueGenerator.Unique<T>(2)
            select new TwoDifferent<T>(items[0], items[1]);
    }

    internal class ThreeDifferentArbitrary<T> : Arbitrary<ThreeDifferent<T>> where T : IEquatable<T>
    {
        public static Arbitrary<ThreeDifferent<T>> Create()
        {
            return new ThreeDifferentArbitrary<T>();
        }

        public override Gen<ThreeDifferent<T>> Generator =>
            from items in UniqueGenerator.Unique<T>(3)
            select new ThreeDifferent<T>(items[0], items[1], items[2]);
    }
}