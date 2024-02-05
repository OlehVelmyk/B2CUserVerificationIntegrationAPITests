using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WX.B2C.User.Verification.Facade.Controllers.Public;

namespace WX.B2C.User.Verification.Api.Public.Helpers
{
    internal static class JwtBearerEventsHandler
    {
        public static Task OnTokenValidated(TokenValidatedContext context) =>
            ValidateClaimsAsync(context);

        private static Task ValidateClaimsAsync(TokenValidatedContext context)
        {
            var ownerId = FindFirst(Constants.ClaimTypes.OwnerId);
            if (ownerId is null)
                context.Fail($"{nameof(Constants.ClaimTypes.OwnerId)} is not found.");
            else if (!Guid.TryParse(ownerId, out _))
                context.Fail($"{nameof(Constants.ClaimTypes.OwnerId)} is not guid.");

            var clientIp = FindFirst(Constants.ClaimTypes.ClientIp);
            if (clientIp is null)
                context.Fail($"{nameof(Constants.ClaimTypes.ClientIp)} is not found.");
            else if (!IPAddress.TryParse(clientIp, out _))
                context.Fail($"{nameof(Constants.ClaimTypes.ClientIp)} is not ip address.");

            return Task.CompletedTask;

            string FindFirst(string claimType) =>
                context.Principal.FindFirstValue(claimType);
        }
    }
}
