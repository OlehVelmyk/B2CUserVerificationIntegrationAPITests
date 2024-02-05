namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.FabricAddressResolver;

public interface IFabricAddressResolver
{
    string ResolveServiceAddress(string serviceName, string loadBalancerAddress);
}