// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Location
    /// </summary>
    /// <remarks>
    /// Locations related to a PEP or sanctioned entity, such as birth place
    /// </remarks>
    public partial class Location
    {
        /// <summary>
        /// Initializes a new instance of the Location class.
        /// </summary>
        public Location()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Location class.
        /// </summary>
        /// <param name="city">City</param>
        /// <param name="country">Country</param>
        /// <param name="region">Region</param>
        /// <param name="address">Address</param>
        /// <param name="type">Location type, e.g. birth place</param>
        public Location(string city = default(string), string country = default(string), string region = default(string), string address = default(string), string type = default(string))
        {
            City = city;
            Country = country;
            Region = region;
            Address = address;
            Type = type;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets city
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets country
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets region
        /// </summary>
        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets address
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets location type, e.g. birth place
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

    }
}