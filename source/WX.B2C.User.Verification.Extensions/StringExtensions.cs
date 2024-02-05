using System;

namespace WX.B2C.User.Verification.Extensions
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string self)
            => Guid.Parse(self);
    }
}
