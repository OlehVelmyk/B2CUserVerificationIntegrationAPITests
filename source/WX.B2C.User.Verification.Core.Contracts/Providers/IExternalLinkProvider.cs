using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IExternalLinkProvider
    {
        string Get(string externalId, ExternalProviderType externalProvider);
    }
}