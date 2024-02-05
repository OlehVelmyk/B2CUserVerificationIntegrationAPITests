using System;
using System.Linq;
using System.Threading;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using Optional;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common
{
    public class FabricAddressResolver : IFabricAddressResolver
    {
        private readonly ILogger _logger;

        public FabricAddressResolver(ILogger logger)
        {
            _logger = logger?.ForContext<FabricAddressResolver>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public Option<string> ResolveServiceAddress(string serviceUrl, string loadBalancerAddress = null)
        {
            try
            {
                var serviceUri = new Uri(serviceUrl);
                var serviceResolver = ServicePartitionResolver.GetDefault();
                var resolvePartitionTask = serviceResolver.ResolveAsync(serviceUri, ServicePartitionKey.Singleton, CancellationToken.None);
                var servicePartition = resolvePartitionTask.GetAwaiter().GetResult();

                var serviceEndpoint = servicePartition.GetEndpoint();
                var addresses = JObject.Parse(serviceEndpoint.Address);
                var serviceAddress = addresses["Endpoints"].First().ToObject<string>();

                _logger.Debug("Internal service url successfully resolved: {Address}", serviceAddress);

                if (string.IsNullOrEmpty(serviceAddress))
                    return Option.None<string>();

                return serviceAddress.Some();
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Could not resolve internal service url");

                if (!string.IsNullOrEmpty(loadBalancerAddress))
                    return loadBalancerAddress.Some();

                return Option.None<string>();
            }
        }
    }
}