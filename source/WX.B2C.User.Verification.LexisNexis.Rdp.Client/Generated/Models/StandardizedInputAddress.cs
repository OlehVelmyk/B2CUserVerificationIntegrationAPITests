// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class StandardizedInputAddress
    {
        /// <summary>
        /// Initializes a new instance of the StandardizedInputAddress class.
        /// </summary>
        public StandardizedInputAddress()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the StandardizedInputAddress class.
        /// </summary>
        /// <param name="streetNumber">Street name.</param>
        /// <param name="streetName">Street name.</param>
        /// <param name="streetSuffix">Street suffix (for example, St or
        /// Ave).</param>
        /// <param name="streetPreDirection">Street pre-direction (for example,
        /// SW or NE).</param>
        /// <param name="unitDesignation">Unit designation (for example, Apt or
        /// Suite).</param>
        /// <param name="unitNumber">Unit number.</param>
        /// <param name="country">County.</param>
        /// <param name="postalCode">Postal code.</param>
        /// <param name="stateCityZip">Two-letter state abbreviation, city, and
        /// ZIP Code.</param>
        /// <param name="lattitude">Latitude coordinates.</param>
        /// <param name="longitude">Longitude coordinates.</param>
        /// <param name="streetPostDirection">Street post-direction (for
        /// example, N or W).</param>
        /// <param name="streetAddress1">Unparsed first address line (for
        /// example, 1 N. Main St).</param>
        /// <param name="streetAddress2">Unparsed second address line (for
        /// example, Unit 3C).</param>
        /// <param name="city">City.</param>
        /// <param name="state">Two-letter state abbreviation (for example, MT
        /// or FL).</param>
        /// <param name="zip4">ZIP+4 Code.</param>
        /// <param name="zip5">ZIP+4 Code.</param>
        public StandardizedInputAddress(string streetNumber = default(string), string streetName = default(string), string streetSuffix = default(string), string streetPreDirection = default(string), string unitDesignation = default(string), string unitNumber = default(string), string country = default(string), string postalCode = default(string), string stateCityZip = default(string), string lattitude = default(string), string longitude = default(string), string streetPostDirection = default(string), string streetAddress1 = default(string), string streetAddress2 = default(string), string city = default(string), string state = default(string), string zip4 = default(string), string zip5 = default(string))
        {
            StreetNumber = streetNumber;
            StreetName = streetName;
            StreetSuffix = streetSuffix;
            StreetPreDirection = streetPreDirection;
            UnitDesignation = unitDesignation;
            UnitNumber = unitNumber;
            Country = country;
            PostalCode = postalCode;
            StateCityZip = stateCityZip;
            Lattitude = lattitude;
            Longitude = longitude;
            StreetPostDirection = streetPostDirection;
            StreetAddress1 = streetAddress1;
            StreetAddress2 = streetAddress2;
            City = city;
            State = state;
            Zip4 = zip4;
            Zip5 = zip5;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets street name.
        /// </summary>
        [JsonProperty(PropertyName = "StreetNumber")]
        public string StreetNumber { get; set; }

        /// <summary>
        /// Gets or sets street name.
        /// </summary>
        [JsonProperty(PropertyName = "StreetName")]
        public string StreetName { get; set; }

        /// <summary>
        /// Gets or sets street suffix (for example, St or Ave).
        /// </summary>
        [JsonProperty(PropertyName = "StreetSuffix")]
        public string StreetSuffix { get; set; }

        /// <summary>
        /// Gets or sets street pre-direction (for example, SW or NE).
        /// </summary>
        [JsonProperty(PropertyName = "StreetPreDirection")]
        public string StreetPreDirection { get; set; }

        /// <summary>
        /// Gets or sets unit designation (for example, Apt or Suite).
        /// </summary>
        [JsonProperty(PropertyName = "UnitDesignation")]
        public string UnitDesignation { get; set; }

        /// <summary>
        /// Gets or sets unit number.
        /// </summary>
        [JsonProperty(PropertyName = "UnitNumber")]
        public string UnitNumber { get; set; }

        /// <summary>
        /// Gets or sets county.
        /// </summary>
        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets postal code.
        /// </summary>
        [JsonProperty(PropertyName = "PostalCode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets two-letter state abbreviation, city, and ZIP Code.
        /// </summary>
        [JsonProperty(PropertyName = "StateCityZip")]
        public string StateCityZip { get; set; }

        /// <summary>
        /// Gets or sets latitude coordinates.
        /// </summary>
        [JsonProperty(PropertyName = "Lattitude")]
        public string Lattitude { get; set; }

        /// <summary>
        /// Gets or sets longitude coordinates.
        /// </summary>
        [JsonProperty(PropertyName = "Longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets street post-direction (for example, N or W).
        /// </summary>
        [JsonProperty(PropertyName = "StreetPostDirection")]
        public string StreetPostDirection { get; set; }

        /// <summary>
        /// Gets or sets unparsed first address line (for example, 1 N. Main
        /// St).
        /// </summary>
        [JsonProperty(PropertyName = "StreetAddress1")]
        public string StreetAddress1 { get; set; }

        /// <summary>
        /// Gets or sets unparsed second address line (for example, Unit 3C).
        /// </summary>
        [JsonProperty(PropertyName = "StreetAddress2")]
        public string StreetAddress2 { get; set; }

        /// <summary>
        /// Gets or sets city.
        /// </summary>
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets two-letter state abbreviation (for example, MT or FL).
        /// </summary>
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets ZIP+4 Code.
        /// </summary>
        [JsonProperty(PropertyName = "Zip4")]
        public string Zip4 { get; set; }

        /// <summary>
        /// Gets or sets ZIP+4 Code.
        /// </summary>
        [JsonProperty(PropertyName = "Zip5")]
        public string Zip5 { get; set; }

    }
}