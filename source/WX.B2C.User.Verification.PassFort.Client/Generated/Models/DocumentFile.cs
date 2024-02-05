// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// DocumentFile
    /// </summary>
    /// <remarks>
    /// A file that is related to the document
    /// </remarks>
    public partial class DocumentFile
    {
        /// <summary>
        /// Initializes a new instance of the DocumentFile class.
        /// </summary>
        public DocumentFile()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentFile class.
        /// </summary>
        /// <param name="type">Possible values include: 'LIVE_VIDEO',
        /// 'VIDEO_FRAME'</param>
        /// <param name="reference">A reference associated with the file, such
        /// as the id of the document stored on a 3rd party
        /// integration.</param>
        public DocumentFile(string fileId, DocumentFileType type, string reference = default(string))
        {
            FileId = fileId;
            Type = type;
            Reference = reference;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "file_id")]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'LIVE_VIDEO', 'VIDEO_FRAME'
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public DocumentFileType Type { get; set; }

        /// <summary>
        /// Gets or sets a reference associated with the file, such as the id
        /// of the document stored on a 3rd party integration.
        /// </summary>
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (FileId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FileId");
            }
        }
    }
}
