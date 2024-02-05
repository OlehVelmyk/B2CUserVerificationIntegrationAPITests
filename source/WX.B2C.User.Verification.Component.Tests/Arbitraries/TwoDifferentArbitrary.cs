using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class TwoDifferentArbitrary<T> : Arbitrary<TwoDifferent<T>> 
    {
        public static Arbitrary<TwoDifferent<T>> Create() => new TwoDifferentArbitrary<T>();

        public override Gen<TwoDifferent<T>> Generator =>
            from items in ArrayGenerator.Unique<T>(2)
            select new TwoDifferent<T>(items[0], items[1]);
    }

    internal class ThreeDifferentArbitrary<T> : Arbitrary<ThreeDifferent<T>>
    {
        public static Arbitrary<ThreeDifferent<T>> Create() => new ThreeDifferentArbitrary<T>();

        public override Gen<ThreeDifferent<T>> Generator =>
            from items in ArrayGenerator.Unique<T>(3)
            select new ThreeDifferent<T>(items[0], items[1], items[2]);
    }
}