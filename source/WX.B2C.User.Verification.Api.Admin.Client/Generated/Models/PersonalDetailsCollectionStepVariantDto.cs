// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    [Newtonsoft.Json.JsonObject("PersonalDetails")]
    public partial class PersonalDetailsCollectionStepVariantDto : CollectionStepVariantDto
    {
        /// <summary>
        /// Initializes a new instance of the
        /// PersonalDetailsCollectionStepVariantDto class.
        /// </summary>
        public PersonalDetailsCollectionStepVariantDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// PersonalDetailsCollectionStepVariantDto class.
        /// </summary>
        /// <param name="property">Possible values include: 'FirstName',
        /// 'LastName', 'Birthdate', 'ResidenceAddress', 'Nationality',
        /// 'Email', 'FullName'</param>
        public PersonalDetailsCollectionStepVariantDto(string name, PersonalDetailsProperty property)
            : base(name)
        {
            Property = property;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'FirstName', 'LastName',
        /// 'Birthdate', 'ResidenceAddress', 'Nationality', 'Email', 'FullName'
        /// </summary>
        [JsonProperty(PropertyName = "property")]
        public PersonalDetailsProperty Property { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}