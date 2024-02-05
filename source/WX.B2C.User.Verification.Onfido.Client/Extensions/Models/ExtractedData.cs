using System;
using Microsoft.Rest.Serialization;

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;

    public partial class ExtractedData
    {
        /// <summary>
        /// Initializes a new instance of the ExtractedData class.
        /// </summary>
        public ExtractedData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ExtractedData class.
        /// </summary>
        /// <param name="firstName">The string value for the firstName. Read-only.</param>
        /// <param name="lastName">The string value for the lastName. Read-only.</param>
        /// <param name="fullName">The string value for the fullName. Read-only.</param>
        /// <param name="dateOfBirth">The string value for the dateOfBirth. Read-only.</param>
        /// <param name="documentNumber">The string value for the documentNumber. Read-only.</param>
        /// <param name="gender">The string value for the gender. Read-only.</param>
        /// <param name="dateOfExpiry">The string value for the dateOfExpiry. Read-only.</param>
        /// <param name="issuingDate">The string value for the issuingDate. Read-only.</param>
        /// <param name="expiryDate">The string value for the expiryDate. Read-only.</param>
        /// <param name="addressLine1">The string value for the addressLine1. Read-only.</param>
        /// <param name="addressLine2">The string value for the addressLine2. Read-only.</param>
        /// <param name="documentType">The string value for the documentType. Read-only.</param>
        /// <param name="issuingCountry">The string value for the issuingCountry. Read-only.</param>
        /// <param name="issuingState">The string value for the issuingState. Read-only.</param>
        /// <param name="nationality">The string value for the nationality. Read-only.</param>
        /// <param name="placeOfBirth">The string value for the placeOfBirth. Read-only.</param>
        /// 
        public ExtractedData(
            string firstName = default(string),
            string lastName = default(string),
            string fullName = default(string),
            DateTime? dateOfBirth = default(DateTime?),
            string documentNumber = default(string),
            string gender = default(string),
            DateTime? dateOfExpiry = default(DateTime?),
            DateTime? issuingDate = default(DateTime?),
            DateTime? expiryDate = default(DateTime?),
            string addressLine1 = default(string),
            string addressLine2 = default(string),
            string documentType = default(string),
            string issuingCountry = default(string),
            string issuingState = default(string),
            string nationality = default(string),
            string placeOfBirth = default(string)
            )
        {
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            DocumentNumber = documentNumber;
            Gender = gender;
            DateOfExpiry = dateOfExpiry;
            IssuingDate = issuingDate;
            ExpiryDate = expiryDate;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            DocumentType = documentType;
            IssuingCountry = issuingCountry;
            IssuingState = issuingState;
            Nationality = nationality;
            PlaceOfBirth = placeOfBirth;

            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for FirstName
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for LastName
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for FullName
        /// </summary>
        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DateOfBirth
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty(PropertyName = "date_of_birth")]
        public System.DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DocumentNumber
        /// </summary>
        [JsonProperty(PropertyName = "document_number")]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for Gender
        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DateOfExpiry
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty(PropertyName = "date_of_expiry")]
        public System.DateTime? DateOfExpiry { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for IssuingDate
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty(PropertyName = "issuing_date")]
        public System.DateTime? IssuingDate { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for ExpiryDate
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty(PropertyName = "expiry_date")]
        public System.DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for AddressLine1
        /// </summary>
        [JsonProperty(PropertyName = "address_line_1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for AddressLine2
        /// </summary>
        [JsonProperty(PropertyName = "address_line_2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DocumentType
        /// </summary>
        [JsonProperty(PropertyName = "document_type")]
        public string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for IssuingCountry
        /// </summary>
        [JsonProperty(PropertyName = "issuing_country")]
        public string IssuingCountry { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for IssuingCountry
        /// </summary>
        [JsonProperty(PropertyName = "issuing_state")]
        public string IssuingState { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for Nationality
        /// </summary>
        [JsonProperty(PropertyName = "nationality")]
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for PlaceOfBirth
        /// </summary>
        [JsonProperty(PropertyName = "place_of_birth")]
        public string PlaceOfBirth { get; set; }
    }
}