using System;
using System.Text.RegularExpressions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    internal static class StringExtensions
    {
        public static bool CompareStrings(this string first, string second)
        {
            if (first is null && second is null)
                return true;
            if (first is null || second is null)
                return false;

            var changed1 = Regex.Replace(first, "[-]|[_]", "");
            var changed2 = Regex.Replace(second, "[-]|[_]", "");
            return changed1.Equals(changed2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
