// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// DocumentBase
    /// </summary>
    /// <remarks>
    /// A single document with one or more images
    /// </remarks>
    public partial class DocumentBase
    {
        /// <summary>
        /// Initializes a new instance of the DocumentBase class.
        /// </summary>
        public DocumentBase()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentBase class.
        /// </summary>
        /// <param name="category">Possible values include:
        /// 'PROOF_OF_IDENTITY', 'PROOF_OF_SOURCE_OF_WEALTH',
        /// 'PROOF_OF_SOURCE_OF_FUNDS', 'PROOF_OF_ADDRESS', 'SUPPORTING',
        /// 'COMPANY_FILING', 'DATA_SUMMARY', 'PROOF_OF_BANK_ACCOUNT',
        /// 'PROOF_OF_TAX_STATUS'</param>
        /// <param name="documentType">Possible values include: 'PASSPORT',
        /// 'PASSPORT_CARD', 'DRIVING_LICENCE', 'STATE_ID', 'VOTER_ID',
        /// 'BIOMETRIC_STATE_ID', 'BIRTH_CERTIFICATE', 'BANK_STATEMENT',
        /// 'FACE_IMAGE', 'UNKNOWN', 'COMPANY_ACCOUNTS',
        /// 'COMPANY_CHANGE_OF_ADDRESS', 'ANNUAL_RETURN',
        /// 'CONFIRMATION_STATEMENT', 'STATEMENT_OF_CAPITAL', 'CHANGE_OF_NAME',
        /// 'INCORPORATION', 'LIQUIDATION', 'MISCELLANEOUS', 'MORTGAGE',
        /// 'CHANGE_OF_OFFICERS', 'RESOLUTION', 'CREDIT_REPORT',
        /// 'CREDIT_CHECK', 'REGISTER_REPORT', 'REGISTER_CHECK',
        /// 'DATA_SUMMARY', 'CHANGE_OF_PSC', 'ADVERSE_MEDIA', 'TAX',
        /// 'UTILITY_BILL', 'HEALTHCARE_ID', 'TAX_ID', 'RESIDENCE_ID',
        /// 'RESIDENCE_OTHER', 'VISA'</param>
        /// <param name="files">Files associated with the document</param>
        public DocumentBase(DocumentCategory category, DocumentType documentType, string id = default(string), DocumentData extractedData = default(DocumentData), DocumentResult verificationResult = default(DocumentResult), IList<DocumentFile> files = default(IList<DocumentFile>))
        {
            Id = id;
            Category = category;
            DocumentType = documentType;
            ExtractedData = extractedData;
            VerificationResult = verificationResult;
            Files = files;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'PROOF_OF_IDENTITY',
        /// 'PROOF_OF_SOURCE_OF_WEALTH', 'PROOF_OF_SOURCE_OF_FUNDS',
        /// 'PROOF_OF_ADDRESS', 'SUPPORTING', 'COMPANY_FILING', 'DATA_SUMMARY',
        /// 'PROOF_OF_BANK_ACCOUNT', 'PROOF_OF_TAX_STATUS'
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public DocumentCategory Category { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'PASSPORT', 'PASSPORT_CARD',
        /// 'DRIVING_LICENCE', 'STATE_ID', 'VOTER_ID', 'BIOMETRIC_STATE_ID',
        /// 'BIRTH_CERTIFICATE', 'BANK_STATEMENT', 'FACE_IMAGE', 'UNKNOWN',
        /// 'COMPANY_ACCOUNTS', 'COMPANY_CHANGE_OF_ADDRESS', 'ANNUAL_RETURN',
        /// 'CONFIRMATION_STATEMENT', 'STATEMENT_OF_CAPITAL', 'CHANGE_OF_NAME',
        /// 'INCORPORATION', 'LIQUIDATION', 'MISCELLANEOUS', 'MORTGAGE',
        /// 'CHANGE_OF_OFFICERS', 'RESOLUTION', 'CREDIT_REPORT',
        /// 'CREDIT_CHECK', 'REGISTER_REPORT', 'REGISTER_CHECK',
        /// 'DATA_SUMMARY', 'CHANGE_OF_PSC', 'ADVERSE_MEDIA', 'TAX',
        /// 'UTILITY_BILL', 'HEALTHCARE_ID', 'TAX_ID', 'RESIDENCE_ID',
        /// 'RESIDENCE_OTHER', 'VISA'
        /// </summary>
        [JsonProperty(PropertyName = "document_type")]
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "extracted_data")]
        public DocumentData ExtractedData { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "verification_result")]
        public DocumentResult VerificationResult { get; set; }

        /// <summary>
        /// Gets or sets files associated with the document
        /// </summary>
        [JsonProperty(PropertyName = "files")]
        public IList<DocumentFile> Files { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Files != null)
            {
                foreach (var element in Files)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
