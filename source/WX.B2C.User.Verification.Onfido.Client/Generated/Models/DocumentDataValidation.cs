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
    /// Asserts whether algorithmically validatable elements are correct.
    /// </summary>
    public partial class DocumentDataValidation : BreakdownResult
    {
        /// <summary>
        /// Initializes a new instance of the DocumentDataValidation class.
        /// </summary>
        public DocumentDataValidation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentDataValidation class.
        /// </summary>
        public DocumentDataValidation(string result = default(string), DocumentDataValidationBreakdown breakdown = default(DocumentDataValidationBreakdown))
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
        public DocumentDataValidationBreakdown Breakdown { get; set; }

    }
}