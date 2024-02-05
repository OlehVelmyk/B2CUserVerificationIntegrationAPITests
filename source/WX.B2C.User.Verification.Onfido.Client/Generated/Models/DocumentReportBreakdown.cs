// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class DocumentReportBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the DocumentReportBreakdown class.
        /// </summary>
        public DocumentReportBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentReportBreakdown class.
        /// </summary>
        public DocumentReportBreakdown(DocumentDataComparison dataComparison = default(DocumentDataComparison), DocumentDataValidation dataValidation = default(DocumentDataValidation), DocumentImageIntegrity imageIntegrity = default(DocumentImageIntegrity), DocumentVisualAuthenticity visualAuthenticity = default(DocumentVisualAuthenticity), DocumentDataConsistency dataConsistency = default(DocumentDataConsistency), DocumentPolicyRecord policeRecord = default(DocumentPolicyRecord), DocumentCompromisedDocument compromisedDocument = default(DocumentCompromisedDocument), DocumentAgeValidation ageValidation = default(DocumentAgeValidation), DocumentIssuingAuthority issuingAuthority = default(DocumentIssuingAuthority))
        {
            DataComparison = dataComparison;
            DataValidation = dataValidation;
            ImageIntegrity = imageIntegrity;
            VisualAuthenticity = visualAuthenticity;
            DataConsistency = dataConsistency;
            PoliceRecord = policeRecord;
            CompromisedDocument = compromisedDocument;
            AgeValidation = ageValidation;
            IssuingAuthority = issuingAuthority;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data_comparison")]
        public DocumentDataComparison DataComparison { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data_validation")]
        public DocumentDataValidation DataValidation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "image_integrity")]
        public DocumentImageIntegrity ImageIntegrity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "visual_authenticity")]
        public DocumentVisualAuthenticity VisualAuthenticity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data_consistency")]
        public DocumentDataConsistency DataConsistency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "police_record")]
        public DocumentPolicyRecord PoliceRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "compromised_document")]
        public DocumentCompromisedDocument CompromisedDocument { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "age_validation")]
        public DocumentAgeValidation AgeValidation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "issuing_authority")]
        public DocumentIssuingAuthority IssuingAuthority { get; set; }

    }
}
