// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// DocumentPost
    /// </summary>
    /// <remarks>
    /// A single document with one or more images
    /// </remarks>
    public partial class DocumentPost : DocumentBase
    {
        /// <summary>
        /// Initializes a new instance of the DocumentPost class.
        /// </summary>
        public DocumentPost()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentPost class.
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
        /// <param name="images">Images that make up the document. For example
        /// a `DRIVING_LICENCE` will have a `FRONT` and a `BACK`</param>
        /// <param name="files">Files associated with the document</param>
        public DocumentPost(DocumentCategory category, DocumentType documentType, IList<DocumentPostImagesItem> images, string id = default(string), DocumentData extractedData = default(DocumentData), DocumentResult verificationResult = default(DocumentResult), IList<DocumentFile> files = default(IList<DocumentFile>))
            : base(category, documentType, id, extractedData, verificationResult, files)
        {
            Images = images;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets images that make up the document. For example a
        /// `DRIVING_LICENCE` will have a `FRONT` and a `BACK`
        /// </summary>
        [JsonProperty(PropertyName = "images")]
        public IList<DocumentPostImagesItem> Images { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Images == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Images");
            }
        }
    }
}