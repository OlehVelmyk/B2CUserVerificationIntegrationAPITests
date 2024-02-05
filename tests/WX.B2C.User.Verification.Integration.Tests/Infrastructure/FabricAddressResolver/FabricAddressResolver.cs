using System.Text.RegularExpressions;
using Microsoft.ServiceFabric.Services.Client;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.FabricAddressResolver;

public class FabricAddressResolver : IFabricAddressResolver
{
    private static readonly Regex _addressParser = new("\"(ServiceEndpoint[V]?[2]?|)\":\"(?<addr>.*)\"");

    public string ResolveServiceAddress(string serviceName, string loadBalancerAddress)
    {
        try
        {
            var serviceEndpoint = ServicePartitionResolver.GetDefault()
                .ResolveAsync(new Uri(serviceName), ServicePartitionKey.Singleton, CancellationToken.None)
                .Result
                .Endpoints?
                .FirstOrDefault(x => x.Address != null)
                ?
                .Address;

            return string.IsNullOrEmpty(serviceEndpoint)
                ? loadBalancerAddress
                : _addressParser.Match(serviceEndpoint)
                    .Groups["addr"]
                    .Captures[0]
                    .Value.Replace("\\/", "/")
                    .Replace("Http", "http");
        }
        catch (Exception) { return loadBalancerAddress; }
    }
}