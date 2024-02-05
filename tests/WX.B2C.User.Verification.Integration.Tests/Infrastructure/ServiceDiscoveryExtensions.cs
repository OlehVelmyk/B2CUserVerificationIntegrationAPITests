using Microsoft.Extensions.DependencyInjection;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.FabricAddressResolver;
using WX.Configuration.ServiceDiscovery.Interface;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure;

public static class ServiceDiscoveryExtensions
{
    public static string GetServiceEndpoint(this IServiceProvider x, string name, bool externalRequired = false)
    {
        var serviceLocator = x.GetRequiredService<IFabricAddressResolver>();
        var serviceEndpoint = x.GetRequiredService<IServiceEndpointsInformation>().ServiceEndpoints[name];

        var result = externalRequired
            ? serviceEndpoint.ExternalEndpoint
            : serviceLocator.ResolveServiceAddress(serviceEndpoint.ServiceId, serviceEndpoint.ExternalEndpoint);

        return result;
    }
}