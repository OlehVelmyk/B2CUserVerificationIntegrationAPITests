using System;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class TwoDifferent<T> : Tuple<T, T>
    {
        public TwoDifferent(T item1, T item2)
            : base(item1, item2)
        {
        }
    }

    internal class ThreeDifferent<T> : Tuple<T, T, T>
    {
        public ThreeDifferent(T item1, T item2, T item3)
            : base(item1, item2, item3)
        {
        }
    }
}
