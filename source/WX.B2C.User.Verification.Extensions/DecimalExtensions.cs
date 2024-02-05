using System;

namespace WX.B2C.User.Verification.Extensions
{
    public static class DecimalExtensions
    {
        public static bool IsEquivalent(this decimal? target, decimal? source)
        {
            if (target == null && source == null)
                return true;
            if (target == null || source == null)
                return false;

            return IsEquivalent(target.Value, source.Value);
        }

        public static bool IsEquivalent(this decimal target, decimal source) => 
            Math.Abs(source - target) < 0.01m;
    }
}
