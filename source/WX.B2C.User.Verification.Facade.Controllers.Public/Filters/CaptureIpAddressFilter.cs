using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Filters
{
    public class CaptureIpAddressAttribute
        : Attribute,
          IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var profileService = serviceProvider.GetRequiredService<IProfileService>();
            var hostSettingsProvider = serviceProvider.GetRequiredService<IHostSettingsProvider>();
            return new CaptureIpAddressFilter(profileService, hostSettingsProvider);
        }
    }

    public class CaptureIpAddressFilter : IAsyncActionFilter
    {
        private const string TestIpAddressHeader = "TestIpAddress";

        private readonly IProfileService _profileService;
        private readonly bool _isProduction;

        public CaptureIpAddressFilter(
            IProfileService profileService, 
            IHostSettingsProvider hostSettingsProvider)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _isProduction = IsProduction(hostSettingsProvider);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.User.GetUserId();
            var clientIpAddress = GetClientIpAddress(httpContext);
            var initiation = InitiationDto.CreateSystem("Set IP address in scope of start verification.");
            var patch = new VerificationDetailsPatch { IpAddress = clientIpAddress.SomeNotNull() };
            await _profileService.UpdateAsync(userId, patch, initiation);

            await next();
        }

        private string GetClientIpAddress(HttpContext httpContext)
        {
            if (_isProduction || !TryGetTestIpAddress(httpContext.Request, out var clientIpAddress))
                clientIpAddress = httpContext.User.GetClientIpAddress();

            return clientIpAddress.ToString();
        }

        private static bool TryGetTestIpAddress(HttpRequest request, out IPAddress ipAddress)
        {
            ipAddress = null;

            var headers = request?.Headers;
            if (headers == null) return false;

            return headers.TryGetValue(TestIpAddressHeader, out var value)
                   && IPAddress.TryParse(value, out ipAddress);
        }

        private static bool IsProduction(IHostSettingsProvider hostSettingsProvider)
        {
            if (hostSettingsProvider == null) throw new ArgumentNullException(nameof(hostSettingsProvider));

            var environment = hostSettingsProvider.GetSetting(HostSettingsKey.Environment);
            return string.Equals(environment, "Production", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}