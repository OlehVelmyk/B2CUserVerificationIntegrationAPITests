// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class AddressLines
    {
        /// <summary>
        /// Initializes a new instance of the AddressLines class.
        /// </summary>
        public AddressLines()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AddressLines class.
        /// </summary>
        public AddressLines(string streetAddress = default(string), string state = default(string), string postalCode = default(string), string country = default(string), string city = default(string), string countryCode = default(string))
        {
            StreetAddress = streetAddress;
            State = state;
            PostalCode = postalCode;
            Country = country;
            City = city;
            CountryCode = countryCode;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "street_address")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

    }
}
