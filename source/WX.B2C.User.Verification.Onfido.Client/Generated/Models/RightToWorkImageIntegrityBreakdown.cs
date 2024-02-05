// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class RightToWorkImageIntegrityBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// RightToWorkImageIntegrityBreakdown class.
        /// </summary>
        public RightToWorkImageIntegrityBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// RightToWorkImageIntegrityBreakdown class.
        /// </summary>
        public RightToWorkImageIntegrityBreakdown(DefaultBreakdownResult imageQuality = default(DefaultBreakdownResult), DefaultBreakdownResult supportedDocument = default(DefaultBreakdownResult), DefaultBreakdownResult colourPicture = default(DefaultBreakdownResult), DefaultBreakdownResult conclusiveDocumentQuality = default(DefaultBreakdownResult))
        {
            ImageQuality = imageQuality;
            SupportedDocument = supportedDocument;
            ColourPicture = colourPicture;
            ConclusiveDocumentQuality = conclusiveDocumentQuality;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "image_quality")]
        public DefaultBreakdownResult ImageQuality { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "supported_document")]
        public DefaultBreakdownResult SupportedDocument { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "colour_picture")]
        public DefaultBreakdownResult ColourPicture { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "conclusive_document_quality")]
        public DefaultBreakdownResult ConclusiveDocumentQuality { get; set; }

    }
}