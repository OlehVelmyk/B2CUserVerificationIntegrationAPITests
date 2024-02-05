using System.Net;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class IpAddressCheckData
    {
        public IPAddress IpAddress { get; set; }

        public AddressDto ResidenceAddress { get; set; }
    }

    internal class IpAddressCheckOutputData : CheckOutputData
    {
        public bool IsIpMatched { get; set; }

        public IpAddressLocation ResolvedLocation { get; set; }
    }
}