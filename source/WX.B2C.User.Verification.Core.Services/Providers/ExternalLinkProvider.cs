using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services
{
    public class ExternalLinkProvider : IExternalLinkProvider
    {
        private static readonly Dictionary<ExternalProviderType, string> LinkTemplates = new()
        {
            [ExternalProviderType.Onfido] = "https://dashboard.onfido.com/library?_sandbox_[0]=false&q={id}",
            [ExternalProviderType.PassFort] = "https://identity.passfort.com/profiles/{id}/applications"
        };

        public string Get(string externalId, ExternalProviderType externalProvider)
        {
            if (!LinkTemplates.TryGetValue(externalProvider, out var template))
                throw new KeyNotFoundException($"Can not find link template for {externalProvider}.");

            return template.Replace("{id}", externalId);
        }
    }
}
