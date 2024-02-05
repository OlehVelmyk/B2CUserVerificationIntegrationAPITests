// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// DocumentData
    /// </summary>
    /// <remarks>
    /// All data extracted from a document
    /// </remarks>
    public partial class DocumentData
    {
        /// <summary>
        /// Initializes a new instance of the DocumentData class.
        /// </summary>
        public DocumentData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentData class.
        /// </summary>
        /// <param name="issuer">Name of the issuing authority</param>
        /// <param name="number">Document number</param>
        /// <param name="mrz1">Line one of the [Machine-Readable
        /// Zone](https://en.wikipedia.org/wiki/Machine-readable_passport)</param>
        /// <param name="mrz2">Line two of the MRZ</param>
        /// <param name="mrz3">Line three of the MRZ</param>
        /// <param name="externalService">External service</param>
        /// <param name="externalRef">External service reference</param>
        public DocumentData(string issuer = default(string), object issuingCountry = default(object), string number = default(string), string issued = default(string), string expiry = default(string), PersonalDetails personalDetails = default(PersonalDetails), IList<DatedAddressHistoryItem> addressHistory = default(IList<DatedAddressHistoryItem>), string mrz1 = default(string), string mrz2 = default(string), string mrz3 = default(string), DocumentResult result = default(DocumentResult), string externalService = default(string), string externalRef = default(string))
        {
            Issuer = issuer;
            IssuingCountry = issuingCountry;
            Number = number;
            Issued = issued;
            Expiry = expiry;
            PersonalDetails = personalDetails;
            AddressHistory = addressHistory;
            Mrz1 = mrz1;
            Mrz2 = mrz2;
            Mrz3 = mrz3;
            Result = result;
            ExternalService = externalService;
            ExternalRef = externalRef;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of the issuing authority
        /// </summary>
        [JsonProperty(PropertyName = "issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "issuing_country")]
        public object IssuingCountry { get; set; }

        /// <summary>
        /// Gets or sets document number
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "issued")]
        public string Issued { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public string Expiry { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "personal_details")]
        public PersonalDetails PersonalDetails { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "address_history")]
        public IList<DatedAddressHistoryItem> AddressHistory { get; set; }

        /// <summary>
        /// Gets or sets line one of the [Machine-Readable
        /// Zone](https://en.wikipedia.org/wiki/Machine-readable_passport)
        /// </summary>
        [JsonProperty(PropertyName = "mrz1")]
        public string Mrz1 { get; set; }

        /// <summary>
        /// Gets or sets line two of the MRZ
        /// </summary>
        [JsonProperty(PropertyName = "mrz2")]
        public string Mrz2 { get; set; }

        /// <summary>
        /// Gets or sets line three of the MRZ
        /// </summary>
        [JsonProperty(PropertyName = "mrz3")]
        public string Mrz3 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public DocumentResult Result { get; set; }

        /// <summary>
        /// Gets or sets external service
        /// </summary>
        [JsonProperty(PropertyName = "external_service")]
        public string ExternalService { get; set; }

        /// <summary>
        /// Gets or sets external service reference
        /// </summary>
        [JsonProperty(PropertyName = "external_ref")]
        public string ExternalRef { get; set; }

    }
}
