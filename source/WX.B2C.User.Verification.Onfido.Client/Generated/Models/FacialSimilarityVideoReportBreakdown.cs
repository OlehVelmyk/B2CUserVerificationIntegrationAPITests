// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class FacialSimilarityVideoReportBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityVideoReportBreakdown class.
        /// </summary>
        public FacialSimilarityVideoReportBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityVideoReportBreakdown class.
        /// </summary>
        public FacialSimilarityVideoReportBreakdown(FacialSimilarityVideoFaceComparison faceComparison = default(FacialSimilarityVideoFaceComparison), FacialSimilarityVideoImageIntegrity imageIntegrity = default(FacialSimilarityVideoImageIntegrity), FacialSimilarityVideoVisualAuthenticity visualAuthenticity = default(FacialSimilarityVideoVisualAuthenticity))
        {
            FaceComparison = faceComparison;
            ImageIntegrity = imageIntegrity;
            VisualAuthenticity = visualAuthenticity;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "face_comparison")]
        public FacialSimilarityVideoFaceComparison FaceComparison { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "image_integrity")]
        public FacialSimilarityVideoImageIntegrity ImageIntegrity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "visual_authenticity")]
        public FacialSimilarityVideoVisualAuthenticity VisualAuthenticity { get; set; }

    }
}
