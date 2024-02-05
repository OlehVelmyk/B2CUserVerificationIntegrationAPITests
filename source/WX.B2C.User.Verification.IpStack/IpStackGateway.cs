using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Rest;
using Optional;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.IpStack.Client;
using WX.B2C.User.Verification.IpStack.Mappers;

namespace WX.B2C.User.Verification.IpStack
{
    internal class IpStackGateway : IIpAddressLocationProvider
    {
        private readonly IIpStackApiClientFactory _clientFactory;
        private readonly IIpAddressLocationMapper _mapper;
        private readonly ILogger _logger;

        public IpStackGateway(
            IIpStackApiClientFactory clientFactory,
            IIpAddressLocationMapper mapper,
            ILogger logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger?.ForContext<IpStackGateway>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Option<IpAddressLocation>> LookupAsync(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException(nameof(ipAddress));

            try
            {
                using var ipStackClient = _clientFactory.Create();
                var ipAddressDetails = await ipStackClient.LookupAsync(ipAddress.ToString());
                if (ipAddressDetails != null)
                {
                    var result = _mapper.Map(ipAddressDetails);
                    return Option.Some(result);
                }
            }
            catch (SerializationException sex)
            {
                var responseContent = sex.Content;
                _logger
                    .ForContext(nameof(responseContent), responseContent)
                    .Error("Unable to deserialize the response from IpStack.");
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Unhandled exception from IpStack.");
            }

            return Option.None<IpAddressLocation>();
        }
    }
}
