// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Internal.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class FacialSimilarityCheckRequest
    {
        /// <summary>
        /// Initializes a new instance of the FacialSimilarityCheckRequest
        /// class.
        /// </summary>
        public FacialSimilarityCheckRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the FacialSimilarityCheckRequest
        /// class.
        /// </summary>
        /// <param name="variant">Possible values include: 'Standard',
        /// 'Video'</param>
        public FacialSimilarityCheckRequest(FacialSimilarityCheckVariant variant)
        {
            Variant = variant;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Standard', 'Video'
        /// </summary>
        [JsonProperty(PropertyName = "variant")]
        public FacialSimilarityCheckVariant Variant { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}