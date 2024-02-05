using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Options;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Api.Admin.Options
{
    internal class ConfigureHttpsRedirectionOptions : IConfigureOptions<HttpsRedirectionOptions>
    {
        private readonly IHostSettingsProvider _hostSettingsProvider;

        public ConfigureHttpsRedirectionOptions(IHostSettingsProvider hostSettingsProvider)
        {
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
        }

        public void Configure(HttpsRedirectionOptions options)
        {
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            options.HttpsPort = int.Parse(_hostSettingsProvider.GetSetting(HostSettingsKey.ApplicationPort));
        }
    }
}