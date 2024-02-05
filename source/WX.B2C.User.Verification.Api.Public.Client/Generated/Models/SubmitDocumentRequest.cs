// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Public.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class SubmitDocumentRequest
    {
        /// <summary>
        /// Initializes a new instance of the SubmitDocumentRequest class.
        /// </summary>
        public SubmitDocumentRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SubmitDocumentRequest class.
        /// </summary>
        /// <param name="category">Possible values include: 'ProofOfIdentity',
        /// 'ProofOfAddress', 'Supporting', 'Taxation', 'ProofOfFunds',
        /// 'Selfie'</param>
        /// <param name="provider">Possible values include: 'Onfido'</param>
        public SubmitDocumentRequest(DocumentCategory category, string type, IList<string> files, ExternalFileProviderType? provider = default(ExternalFileProviderType?))
        {
            Category = category;
            Type = type;
            Provider = provider;
            Files = files;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'ProofOfIdentity',
        /// 'ProofOfAddress', 'Supporting', 'Taxation', 'ProofOfFunds',
        /// 'Selfie'
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public DocumentCategory Category { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Onfido'
        /// </summary>
        [JsonProperty(PropertyName = "provider")]
        public ExternalFileProviderType? Provider { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "files")]
        public IList<string> Files { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Type == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Type");
            }
            if (Files == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Files");
            }
        }
    }
}