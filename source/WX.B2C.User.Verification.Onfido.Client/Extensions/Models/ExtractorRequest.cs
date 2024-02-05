namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;

    public partial class ExtractionRequest
    {
        /// <summary>
        /// Initializes a new instance of the ExtractorRequest class.
        /// </summary>
        public ExtractionRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ExtractorRequest class.
        /// </summary>
        /// <param name="documentId">The unique identifier of the documentId
        /// </param>
        public ExtractionRequest(string documentId)
        {
            DocumentId = documentId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the unique identifier of the document
        /// </summary>
        [JsonProperty(PropertyName = "document_id")]
        public string DocumentId { get; set; }
    }
}
