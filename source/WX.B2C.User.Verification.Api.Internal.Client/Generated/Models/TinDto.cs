// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Internal.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class TinDto
    {
        /// <summary>
        /// Initializes a new instance of the TinDto class.
        /// </summary>
        public TinDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TinDto class.
        /// </summary>
        /// <param name="type">Possible values include: 'SSN', 'ITIN'</param>
        public TinDto(string number, TinType type)
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
        /// Gets or sets possible values include: 'SSN', 'ITIN'
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public TinType Type { get; set; }

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
        }
    }
}