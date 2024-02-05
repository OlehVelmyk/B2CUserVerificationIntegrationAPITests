// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class FacialSimilarityVideoVisualAuthenticityBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityVideoVisualAuthenticityBreakdown class.
        /// </summary>
        public FacialSimilarityVideoVisualAuthenticityBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// FacialSimilarityVideoVisualAuthenticityBreakdown class.
        /// </summary>
        /// <param name="livenessDetected">Asserts whether the numbers and head
        /// movements were correctly executed.</param>
        public FacialSimilarityVideoVisualAuthenticityBreakdown(DefaultBreakdownResult livenessDetected = default(DefaultBreakdownResult), FacialSimilarityVideoSpoofingDetection spoofingDetection = default(FacialSimilarityVideoSpoofingDetection))
        {
            LivenessDetected = livenessDetected;
            SpoofingDetection = spoofingDetection;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets asserts whether the numbers and head movements were
        /// correctly executed.
        /// </summary>
        [JsonProperty(PropertyName = "liveness_detected")]
        public DefaultBreakdownResult LivenessDetected { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "spoofing_detection")]
        public FacialSimilarityVideoSpoofingDetection SpoofingDetection { get; set; }

    }
}
