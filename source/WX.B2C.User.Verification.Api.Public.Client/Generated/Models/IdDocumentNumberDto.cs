// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Public.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IdDocumentNumberDto
    {
        /// <summary>
        /// Initializes a new instance of the IdDocumentNumberDto class.
        /// </summary>
        public IdDocumentNumberDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IdDocumentNumberDto class.
        /// </summary>
        public IdDocumentNumberDto(string number, string type)
        {
            Number = number;
            Type = type;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Number == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Number");
            }
            if (Type == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Type");
            }
        }
    }
}
