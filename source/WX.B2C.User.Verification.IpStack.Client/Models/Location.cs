// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.IpStack.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An object containing location details associated with the IP.
    /// </summary>
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
        /// <param name="geonameId">Unique geoname identifier in accordance
        /// with the Geonames Registry. (http://www.geonames.org/)</param>
        /// <param name="capital">Capital city of the country associated with
        /// the IP.</param>
        /// <param name="languages">An object containing one or multiple
        /// sub-objects per language spoken in the country associated with the
        /// IP.</param>
        /// <param name="countryFlag">HTTP URL leading to an SVG-flag icon for
        /// the country associated with the IP.</param>
        /// <param name="countryFlagEmoji">Emoji icon for the flag of the
        /// country associated with the IP.</param>
        /// <param name="countryFlagEmojiUnicode">Unicode value of the emoji
        /// icon for the flag of the country associated with the IP. (e.g.
        /// U+1F1F5 U+1F1F9 for the Portuguese flag).</param>
        /// <param name="callingCode">Calling/dial code of the country
        /// associated with the IP. (e.g. 351) for Portugal.</param>
        /// <param name="isEu">True or false depending on whether or not the
        /// county associated with the IP is in the European Union.</param>
        public Location(double geonameId = default(double), string capital = default(string), IList<Language> languages = default(IList<Language>), string countryFlag = default(string), string countryFlagEmoji = default(string), string countryFlagEmojiUnicode = default(string), string callingCode = default(string), bool isEu = default(bool))
        {
            GeonameId = geonameId;
            Capital = capital;
            Languages = languages;
            CountryFlag = countryFlag;
            CountryFlagEmoji = countryFlagEmoji;
            CountryFlagEmojiUnicode = countryFlagEmojiUnicode;
            CallingCode = callingCode;
            IsEu = isEu;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets unique geoname identifier in accordance with the Geonames
        /// Registry. (http://www.geonames.org/)
        /// </summary>
        [JsonProperty(PropertyName = "geoname_id")]
        public double GeonameId { get; private set; }

        /// <summary>
        /// Gets capital city of the country associated with the IP.
        /// </summary>
        [JsonProperty(PropertyName = "capital")]
        public string Capital { get; private set; }

        /// <summary>
        /// Gets an object containing one or multiple sub-objects per language
        /// spoken in the country associated with the IP.
        /// </summary>
        [JsonProperty(PropertyName = "languages")]
        public IList<Language> Languages { get; private set; }

        /// <summary>
        /// Gets HTTP URL leading to an SVG-flag icon for the country
        /// associated with the IP.
        /// </summary>
        [JsonProperty(PropertyName = "country_flag")]
        public string CountryFlag { get; private set; }

        /// <summary>
        /// Gets emoji icon for the flag of the country associated with the IP.
        /// </summary>
        [JsonProperty(PropertyName = "country_flag_emoji")]
        public string CountryFlagEmoji { get; private set; }

        /// <summary>
        /// Gets unicode value of the emoji icon for the flag of the country
        /// associated with the IP. (e.g. U+1F1F5 U+1F1F9 for the Portuguese
        /// flag).
        /// </summary>
        [JsonProperty(PropertyName = "country_flag_emoji_unicode")]
        public string CountryFlagEmojiUnicode { get; private set; }

        /// <summary>
        /// Gets calling/dial code of the country associated with the IP. (e.g.
        /// 351) for Portugal.
        /// </summary>
        [JsonProperty(PropertyName = "calling_code")]
        public string CallingCode { get; private set; }

        /// <summary>
        /// Gets true or false depending on whether or not the county
        /// associated with the IP is in the European Union.
        /// </summary>
        [JsonProperty(PropertyName = "is_eu")]
        public bool IsEu { get; private set; }

    }
}