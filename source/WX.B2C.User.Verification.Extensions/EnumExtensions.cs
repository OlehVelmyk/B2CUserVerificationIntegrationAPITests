using System;

namespace WX.B2C.User.Verification.Extensions
{
    public static class EnumExtensions
    {
        public static TResult To<TResult>(this Enum self)
            where TResult : struct, Enum
            => Enum.Parse<TResult>(self.ToString());

        public static TResult To<TResult>(this string self)
            where TResult : struct, Enum
            => Enum.Parse<TResult>(self);

        public static TResult To<TResult>(this int self)
            where TResult : struct, Enum
            => Enum.GetName(typeof(TResult), self)
                   .To<TResult>();

        public static TResult To<TResult>(this long self)
            where TResult : struct, Enum
            => Enum.GetName(typeof(TResult), self)
                   .To<TResult>();

        public static TResult? As<TResult>(this Enum self)
            where TResult : struct, Enum
            => self?.To<TResult>();

        public static TResult? As<TResult>(this string self)
            where TResult : struct, Enum
            => self?.To<TResult>();

        public static TResult? As<TResult>(this int? self)
            where TResult : struct, Enum
            => self?.To<TResult>();
    }
}