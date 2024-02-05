using System;
using System.Net;
using System.Security.Claims;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claims)
        {
            const string claimType = Constants.ClaimTypes.OwnerId;

            var claimValue = GetValue(claims, claimType);

            if (!Guid.TryParse(claimValue, out var userId))
                throw new ArgumentException($"Invalid format of claim {claimType}:{claimValue}.");

            return userId;
        }

        public static IPAddress GetClientIpAddress(this ClaimsPrincipal claims)
        {
            const string claimType = Constants.ClaimTypes.ClientIp;

            var claimValue = GetValue(claims, claimType);

            if (!IPAddress.TryParse(claimValue, out var clientIp))
                throw new ArgumentException($"Invalid format of claim {claimType}:{claimValue}.");

            return clientIp;
        }

        private static string GetValue(ClaimsPrincipal claims, string claimType)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var claimValue = claims.FindFirstValue(claimType);
            if (claimValue == null)
                throw new ArgumentException($"Not found claim of type {claimType}.");

            return claimValue;
        }
    }
}
