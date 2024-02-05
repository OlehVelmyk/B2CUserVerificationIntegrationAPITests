// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class DocumentVisualAuthenticityBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// DocumentVisualAuthenticityBreakdown class.
        /// </summary>
        public DocumentVisualAuthenticityBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// DocumentVisualAuthenticityBreakdown class.
        /// </summary>
        /// <param name="fonts">Fonts in the document don’t match the expected
        /// ones.</param>
        /// <param name="pictureFaceIntegrity">The pictures of the person
        /// identified on the document show signs of tampering or
        /// alteration.</param>
        /// <param name="template">The document doesn’t match the expected
        /// template for the document type and country it is from.</param>
        /// <param name="securityFeatures">Security features expected on the
        /// document are missing or wrong.</param>
        /// <param name="digitalTampering">Indication of digital tampering in
        /// the image.</param>
        /// <param name="other">This sub-breakdown is returned for backward
        /// compatibility reasons. Its value will be consider when at least one
        /// of the other breakdowns is consider, and clear when all the other
        /// breakdowns are clear.</param>
        /// <param name="faceDetection">No face was detected on the
        /// document.</param>
        public DocumentVisualAuthenticityBreakdown(DefaultBreakdownResult fonts = default(DefaultBreakdownResult), DefaultBreakdownResult pictureFaceIntegrity = default(DefaultBreakdownResult), DefaultBreakdownResult template = default(DefaultBreakdownResult), DefaultBreakdownResult securityFeatures = default(DefaultBreakdownResult), DocumentOriginalDocumentPresent originalDocumentPresent = default(DocumentOriginalDocumentPresent), DefaultBreakdownResult digitalTampering = default(DefaultBreakdownResult), DefaultBreakdownResult other = default(DefaultBreakdownResult), DefaultBreakdownResult faceDetection = default(DefaultBreakdownResult))
        {
            Fonts = fonts;
            PictureFaceIntegrity = pictureFaceIntegrity;
            Template = template;
            SecurityFeatures = securityFeatures;
            OriginalDocumentPresent = originalDocumentPresent;
            DigitalTampering = digitalTampering;
            Other = other;
            FaceDetection = faceDetection;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets fonts in the document don’t match the expected ones.
        /// </summary>
        [JsonProperty(PropertyName = "fonts")]
        public DefaultBreakdownResult Fonts { get; set; }

        /// <summary>
        /// Gets or sets the pictures of the person identified on the document
        /// show signs of tampering or alteration.
        /// </summary>
        [JsonProperty(PropertyName = "picture_face_integrity")]
        public DefaultBreakdownResult PictureFaceIntegrity { get; set; }

        /// <summary>
        /// Gets or sets the document doesn’t match the expected template for
        /// the document type and country it is from.
        /// </summary>
        [JsonProperty(PropertyName = "template")]
        public DefaultBreakdownResult Template { get; set; }

        /// <summary>
        /// Gets or sets security features expected on the document are missing
        /// or wrong.
        /// </summary>
        [JsonProperty(PropertyName = "security_features")]
        public DefaultBreakdownResult SecurityFeatures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "original_document_present")]
        public DocumentOriginalDocumentPresent OriginalDocumentPresent { get; set; }

        /// <summary>
        /// Gets or sets indication of digital tampering in the image.
        /// </summary>
        [JsonProperty(PropertyName = "digital_tampering")]
        public DefaultBreakdownResult DigitalTampering { get; set; }

        /// <summary>
        /// Gets or sets this sub-breakdown is returned for backward
        /// compatibility reasons. Its value will be consider when at least one
        /// of the other breakdowns is consider, and clear when all the other
        /// breakdowns are clear.
        /// </summary>
        [JsonProperty(PropertyName = "other")]
        public DefaultBreakdownResult Other { get; set; }

        /// <summary>
        /// Gets or sets no face was detected on the document.
        /// </summary>
        [JsonProperty(PropertyName = "face_detection")]
        public DefaultBreakdownResult FaceDetection { get; set; }

    }
}
