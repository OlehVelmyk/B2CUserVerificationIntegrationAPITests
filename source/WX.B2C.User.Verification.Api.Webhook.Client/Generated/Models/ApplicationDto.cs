// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Webhook.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ApplicationDto
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDto class.
        /// </summary>
        public ApplicationDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationDto class.
        /// </summary>
        public ApplicationDto(string id, ProductDto product, string status)
        {
            Id = id;
            Product = product;
            Status = status;
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
        /// </summary>
        [JsonProperty(PropertyName = "product")]
        public ProductDto Product { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Id");
            }
            if (Product == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Product");
            }
            if (Status == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Status");
            }
            if (Product != null)
            {
                Product.Validate();
            }
        }
    }
}