using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Extensions
{
    internal static class ReflectionExtensions
    {
        public static bool IsActionResult(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeof(IActionResult).IsAssignableFrom(type) || typeof(IConvertToActionResult).IsAssignableFrom(type);
        }

        public static bool IsStringEnum(this ICustomAttributeProvider provider)
            => provider?.IsDefined(typeof(StringEnumAttribute), false) ?? false;

        public static bool IsOptional(this ICustomAttributeProvider provider)
            => provider?.IsDefined(typeof(NotRequiredAttribute), false) ?? false;
    }
}
