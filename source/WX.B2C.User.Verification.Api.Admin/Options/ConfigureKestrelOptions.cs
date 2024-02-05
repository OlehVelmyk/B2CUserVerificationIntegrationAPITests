using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using WX.KeyVault;

namespace WX.B2C.User.Verification.Api.Admin.Options
{
    internal class ConfigureKestrelOptions : IConfigureOptions<KestrelServerOptions>
    {
        private readonly X509Certificate2 _serverCertificate;

        public ConfigureKestrelOptions(KeyVaultConfiguration keyVaultConfiguration)
        {
            var certificateStorage = new CachedCertificateStorage(keyVaultConfiguration);
            _serverCertificate = certificateStorage.GetCertificateByName("wx-admin-server-certificate");
        }

        public void Configure(KestrelServerOptions options)
        {
            options.Limits.MinRequestBodyDataRate = null;
            options.Limits.MinResponseDataRate = null;
            options.ConfigureHttpsDefaults(adapterOptions =>
            {
                adapterOptions.ServerCertificate = _serverCertificate;
            });
        }
    }
}