namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;

    public partial class Extraction
    {
        /// <summary>
        /// Initializes a new instance of the Extractor class.
        /// </summary>
        public Extraction()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Extractor class.
        /// </summary>
        /// <param name="documentId">The unique identifier for the extractor. Read-only.</param>
        /// <param name="documentClassification">The document classification. Read-only.</param>
        /// <param name="extractedData">The extracted data. Read-only.</param>

        public Extraction(string documentId = default(string), DocumentClassification documentClassification = default(DocumentClassification), ExtractedData extractedData = default(ExtractedData))
        {
            DocumentId = documentId;
            DocumentClassification = documentClassification;
            ExtractedData = extractedData;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DocumentId
        /// </summary>
        [JsonProperty(PropertyName = "document_id")]
        public string DocumentId { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for DocumentClassification
        /// </summary>
        [JsonProperty(PropertyName = "document_classification")]
        public DocumentClassification DocumentClassification { get; set; }

        /// <summary>
        /// Gets or sets triggers a pre-determined sub-result response for ExtractedData
        /// </summary>
        [JsonProperty(PropertyName = "extracted_data")]
        public ExtractedData ExtractedData { get; set; }
    }
}
