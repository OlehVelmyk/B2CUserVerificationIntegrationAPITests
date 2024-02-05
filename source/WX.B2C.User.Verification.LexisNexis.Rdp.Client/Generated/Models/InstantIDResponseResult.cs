// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class InstantIDResponseResult
    {
        /// <summary>
        /// Initializes a new instance of the InstantIDResponseResult class.
        /// </summary>
        public InstantIDResponseResult()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InstantIDResponseResult class.
        /// </summary>
        /// <param name="dobVerified">Indicates whether the DOB is
        /// verified.</param>
        /// <param name="nameAddressSsnSummary">Index that indicates the level
        /// of the match of the submitted NAS.</param>
        /// <param name="ssnFoundForLexID">Indicates whether an SSN is found in
        /// the input individual's LexID record.</param>
        /// <param name="addressPOBox">Indicates whether the address is a PO
        /// Box.</param>
        /// <param name="addressCMRA">Indicates whether the address is a
        /// CMRA.</param>
        /// <param name="instantIdVersion">InstantID version that is used to
        /// generate the results.</param>
        /// <param name="emergingId">Indicates whether the NAS verification
        /// record is a LexisNexis Risk Solutions emerging identity record and
        /// does not have an assigned LexID number.</param>
        /// <param name="addressStandardized">Indicates whether the information
        /// that is returned in the StandardizedInputAddress response structure
        /// is standardized (based on USPS guidelines).</param>
        public InstantIDResponseResult(InputEcho inputEcho = default(InputEcho), string uniqueId = default(string), VerifiedInput verifiedInput = default(VerifiedInput), bool? dobVerified = default(bool?), int? nameAddressSsnSummary = default(int?), NameAddressPhone nameAddressPhone = default(NameAddressPhone), ComprehensiveVerification comprehensiveVerification = default(ComprehensiveVerification), string additionalScore1 = default(string), string additionalScore2 = default(string), bool? ssnFoundForLexID = default(bool?), bool? addressPOBox = default(bool?), bool? addressCMRA = default(bool?), string instantIdVersion = default(string), bool? emergingId = default(bool?), bool? addressStandardized = default(bool?), StandardizedInputAddress standardizedInputAddress = default(StandardizedInputAddress))
        {
            InputEcho = inputEcho;
            UniqueId = uniqueId;
            VerifiedInput = verifiedInput;
            DobVerified = dobVerified;
            NameAddressSsnSummary = nameAddressSsnSummary;
            NameAddressPhone = nameAddressPhone;
            ComprehensiveVerification = comprehensiveVerification;
            AdditionalScore1 = additionalScore1;
            AdditionalScore2 = additionalScore2;
            SsnFoundForLexID = ssnFoundForLexID;
            AddressPOBox = addressPOBox;
            AddressCMRA = addressCMRA;
            InstantIdVersion = instantIdVersion;
            EmergingId = emergingId;
            AddressStandardized = addressStandardized;
            StandardizedInputAddress = standardizedInputAddress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "InputEcho")]
        public InputEcho InputEcho { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "UniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "VerifiedInput")]
        public VerifiedInput VerifiedInput { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the DOB is verified.
        /// </summary>
        [JsonProperty(PropertyName = "DobVerified")]
        public bool? DobVerified { get; set; }

        /// <summary>
        /// Gets or sets index that indicates the level of the match of the
        /// submitted NAS.
        /// </summary>
        [JsonProperty(PropertyName = "NameAddressSsnSummary")]
        public int? NameAddressSsnSummary { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "NameAddressPhone")]
        public NameAddressPhone NameAddressPhone { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ComprehensiveVerification")]
        public ComprehensiveVerification ComprehensiveVerification { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AdditionalScore1")]
        public string AdditionalScore1 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AdditionalScore2")]
        public string AdditionalScore2 { get; set; }

        /// <summary>
        /// Gets or sets indicates whether an SSN is found in the input
        /// individual's LexID record.
        /// </summary>
        [JsonProperty(PropertyName = "SsnFoundForLexID")]
        public bool? SsnFoundForLexID { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the address is a PO Box.
        /// </summary>
        [JsonProperty(PropertyName = "AddressPOBox")]
        public bool? AddressPOBox { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the address is a CMRA.
        /// </summary>
        [JsonProperty(PropertyName = "AddressCMRA")]
        public bool? AddressCMRA { get; set; }

        /// <summary>
        /// Gets or sets instantID version that is used to generate the
        /// results.
        /// </summary>
        [JsonProperty(PropertyName = "InstantIdVersion")]
        public string InstantIdVersion { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the NAS verification record is a
        /// LexisNexis Risk Solutions emerging identity record and does not
        /// have an assigned LexID number.
        /// </summary>
        [JsonProperty(PropertyName = "EmergingId")]
        public bool? EmergingId { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the information that is returned in
        /// the StandardizedInputAddress response structure is standardized
        /// (based on USPS guidelines).
        /// </summary>
        [JsonProperty(PropertyName = "AddressStandardized")]
        public bool? AddressStandardized { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "StandardizedInputAddress")]
        public StandardizedInputAddress StandardizedInputAddress { get; set; }

    }
}
