using System;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData
{
    internal class Two<T> : Tuple<T, T>
    {
        public Two(T item1, T item2)
            : base(item1, item2)
        {
        }
    }

    internal class Three<T> : Tuple<T, T, T>
    {
        public Three(T item1, T item2, T item3)
            : base(item1, item2, item3)
        {
        }
    }
}
