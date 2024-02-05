// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Asserts whether data on the document is consistent with data provided
    /// when creating an applicant through the API.
    /// </summary>
    public partial class DocumentDataComparison : BreakdownResult
    {
        /// <summary>
        /// Initializes a new instance of the DocumentDataComparison class.
        /// </summary>
        public DocumentDataComparison()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentDataComparison class.
        /// </summary>
        public DocumentDataComparison(string result = default(string), DocumentDataComparisonBreakdown breakdown = default(DocumentDataComparisonBreakdown))
            : base(result)
        {
            Breakdown = breakdown;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "breakdown")]
        public DocumentDataComparisonBreakdown Breakdown { get; set; }

    }
}