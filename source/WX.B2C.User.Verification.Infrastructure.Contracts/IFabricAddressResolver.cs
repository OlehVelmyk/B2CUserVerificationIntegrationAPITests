using Optional;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IFabricAddressResolver
    {
        Option<string> ResolveServiceAddress(string serviceUrl, string loadBalancerAddress = null);
    }
}