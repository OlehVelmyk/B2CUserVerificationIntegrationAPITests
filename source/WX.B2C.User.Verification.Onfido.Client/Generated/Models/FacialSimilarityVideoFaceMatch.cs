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
    /// Contains a score value under the properties bag.
    /// </summary>
    public partial class FacialSimilarityVideoFaceMatch : BreakdownResult
    {
        /// <summary>
        /// Initializes a new instance of the FacialSimilarityVideoFaceMatch
        /// class.
        /// </summary>
        public FacialSimilarityVideoFaceMatch()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the FacialSimilarityVideoFaceMatch
        /// class.
        /// </summary>
        public FacialSimilarityVideoFaceMatch(string result = default(string), FacialSimilarityVideoFaceMatchProperties properties = default(FacialSimilarityVideoFaceMatchProperties))
            : base(result)
        {
            Properties = properties;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public FacialSimilarityVideoFaceMatchProperties Properties { get; set; }

    }
}
