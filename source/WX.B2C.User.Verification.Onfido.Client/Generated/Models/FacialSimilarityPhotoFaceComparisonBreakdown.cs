// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class FacialSimilarityPhotoFaceComparisonBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityPhotoFaceComparisonBreakdown class.
        /// </summary>
        public FacialSimilarityPhotoFaceComparisonBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityPhotoFaceComparisonBreakdown class.
        /// </summary>
        public FacialSimilarityPhotoFaceComparisonBreakdown(FacialSimilarityPhotoFaceMatch faceMatch = default(FacialSimilarityPhotoFaceMatch))
        {
            FaceMatch = faceMatch;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "face_match")]
        public FacialSimilarityPhotoFaceMatch FaceMatch { get; set; }

    }
}
