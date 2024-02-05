using System;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.IpStack.Mappers
{
    internal interface IIpAddressLocationMapper
    {
        IpAddressLocation Map(Client.Models.IpAddressDetails ipAddressDetails);
    }

    internal class IpAddressLocationMapper : IIpAddressLocationMapper
    {
        public IpAddressLocation Map(Client.Models.IpAddressDetails ipAddressDetails)
        {
            if (ipAddressDetails == null)
                throw new ArgumentNullException(nameof(ipAddressDetails));

            return new IpAddressLocation
            {
                ContinentCode = ipAddressDetails.ContinentCode,
                ContinentName = ipAddressDetails.ContinentName,
                CountryName = ipAddressDetails.CountryName,
                CountryCode = ipAddressDetails.CountryCode,
                City = ipAddressDetails.City,
                Zip = ipAddressDetails.Zip,
                Latitude = ipAddressDetails.Latitude,
                Longitude = ipAddressDetails.Longitude,
                StateCode = ipAddressDetails.RegionCode,
                StateName = ipAddressDetails.RegionName
            };
        }
    }
}
