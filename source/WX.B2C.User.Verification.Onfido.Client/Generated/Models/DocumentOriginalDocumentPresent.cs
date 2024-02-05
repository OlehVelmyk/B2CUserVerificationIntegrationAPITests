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
    /// The document was not present when the photo was taken.
    /// </summary>
    public partial class DocumentOriginalDocumentPresent : BreakdownResult
    {
        /// <summary>
        /// Initializes a new instance of the DocumentOriginalDocumentPresent
        /// class.
        /// </summary>
        public DocumentOriginalDocumentPresent()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentOriginalDocumentPresent
        /// class.
        /// </summary>
        public DocumentOriginalDocumentPresent(string result = default(string), DocumentOriginalDocumentPresentProperties properties = default(DocumentOriginalDocumentPresentProperties))
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
        public DocumentOriginalDocumentPresentProperties Properties { get; set; }

    }
}