using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Optional;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Core.Contracts
{    /// <summary>
    /// Represents IP address location details.
    /// </summary>
    public class IpAddressLocation : ValueObject
    {
        public string ContinentCode { get; set; }

        public string ContinentName { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public string StateName { get; set; }

        public string StateCode { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ContinentCode;
            yield return ContinentName;
            yield return CountryName;
            yield return CountryCode;
            yield return StateName;
            yield return StateCode;
            yield return City;
            yield return Zip;
            yield return Latitude;
            yield return Longitude;
        }
    }

    public interface IIpAddressLocationProvider
    {
        /// <summary>
        /// Locates IP address and returns IP address location details.
        /// </summary>
        /// <param name="ipAddress">IP address to be located.</param>
        /// <returns>IP address location details.</returns>
        Task<Option<IpAddressLocation>> LookupAsync(IPAddress ipAddress);
    }
}
