// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class RightToWorkDataComparisonBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// RightToWorkDataComparisonBreakdown class.
        /// </summary>
        public RightToWorkDataComparisonBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// RightToWorkDataComparisonBreakdown class.
        /// </summary>
        public RightToWorkDataComparisonBreakdown(DefaultBreakdownResult issuingCountry = default(DefaultBreakdownResult), DefaultBreakdownResult gender = default(DefaultBreakdownResult), DefaultBreakdownResult dateOfExpiry = default(DefaultBreakdownResult), DefaultBreakdownResult lastName = default(DefaultBreakdownResult), DefaultBreakdownResult documentType = default(DefaultBreakdownResult), DefaultBreakdownResult documentNumbers = default(DefaultBreakdownResult), DefaultBreakdownResult firstName = default(DefaultBreakdownResult), DefaultBreakdownResult dateOfBirth = default(DefaultBreakdownResult))
        {
            IssuingCountry = issuingCountry;
            Gender = gender;
            DateOfExpiry = dateOfExpiry;
            LastName = lastName;
            DocumentType = documentType;
            DocumentNumbers = documentNumbers;
            FirstName = firstName;
            DateOfBirth = dateOfBirth;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "issuing_country")]
        public DefaultBreakdownResult IssuingCountry { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public DefaultBreakdownResult Gender { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "date_of_expiry")]
        public DefaultBreakdownResult DateOfExpiry { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public DefaultBreakdownResult LastName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "document_type")]
        public DefaultBreakdownResult DocumentType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "document_numbers")]
        public DefaultBreakdownResult DocumentNumbers { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public DefaultBreakdownResult FirstName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "date_of_birth")]
        public DefaultBreakdownResult DateOfBirth { get; set; }

    }
}