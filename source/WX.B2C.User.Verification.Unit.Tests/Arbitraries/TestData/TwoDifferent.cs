using System;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData
{
    internal class TwoDifferent<T> : Two<T> where T : IEquatable<T>
    {
        public TwoDifferent(T item1, T item2)
            : base(item1, item2)
        {
        }
    }

    internal class ThreeDifferent<T> : Three<T> where T : IEquatable<T>
    {
        public ThreeDifferent(T item1, T item2, T item3)
            : base(item1, item2, item3)
        {
        }
    }
}
