using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Component.Tests.Factories
{
    // TODO: Reimplement token regeneration after expiration
    internal class AdministratorFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ITestKeyVault _keyVault;
        private AsyncLazy<Administrator> _lazyAdministrator;

        public AdministratorFactory(IConfiguration configuration, ITestKeyVault keyVault)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _keyVault = keyVault ?? throw new ArgumentNullException(nameof(keyVault));
            _lazyAdministrator = new(CreateTopSecurityAdminInternalAsync);
        }

        public async Task<Administrator> CreateTopSecurityAdminAsync()
        {
            var admin = await _lazyAdministrator.Value;
            if (admin.ExpiresAt > DateTime.UtcNow)
                return admin;

            _lazyAdministrator = new(CreateTopSecurityAdminInternalAsync);
            return await _lazyAdministrator.Value;
        }

        private Task<Administrator> CreateTopSecurityAdminInternalAsync()
        {
            var url = GetUrl();
            var clientId = _configuration["ActiveDirectoryConfig:ClientId"];

            var parameters = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "resource", clientId },
                { "username", _configuration["AdminCredentials:TopSecurity:Email"] },
                { "password", _configuration["AdminCredentials:TopSecurity:Password"] },
                { "client_assertion", GetClientAssertion(url, clientId) },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" }
            };

            return Administrator.LoginAsync(url, parameters);
        }

        private string GetUrl()
        {
            var tenantId = _configuration["ActiveDirectoryConfig:TenantId"];
            return $"https://login.microsoftonline.com/{tenantId}/oauth2/token";
        }

        /// <summary>
        /// Returns signed JWT
        /// </summary>
        private string GetClientAssertion(string url, string clientId)
        {
            //Requirements: https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-certificate-credentials
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = clientId,
                Audience = url,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", clientId),
                    new Claim("jti", Guid.NewGuid().ToString())
                }),
                SigningCredentials = GetSigningCertificate()
            };

            var handler = new JwtSecurityTokenHandler();
            var token = (JwtSecurityToken)handler.CreateToken(securityTokenDescriptor);
            return token.RawData;
        }

        private X509SigningCredentials GetSigningCertificate()
        {
            var base64 = _keyVault.AdminPanelApplicationCertificate.UnSecure();
            var certificate = new X509Certificate2(Convert.FromBase64String(base64));
            return new X509SigningCredentials(certificate);
        }
    }
}