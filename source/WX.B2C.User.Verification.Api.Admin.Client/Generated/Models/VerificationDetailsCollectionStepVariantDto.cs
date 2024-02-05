// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    [Newtonsoft.Json.JsonObject("VerificationDetails")]
    public partial class VerificationDetailsCollectionStepVariantDto : CollectionStepVariantDto
    {
        /// <summary>
        /// Initializes a new instance of the
        /// VerificationDetailsCollectionStepVariantDto class.
        /// </summary>
        public VerificationDetailsCollectionStepVariantDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// VerificationDetailsCollectionStepVariantDto class.
        /// </summary>
        /// <param name="property">Possible values include: 'IpAddress',
        /// 'TaxResidence', 'RiskLevel', 'IdDocumentNumber', 'Tin',
        /// 'Nationality', 'IsPep', 'IsSanctioned', 'IsAdverseMedia',
        /// 'Turnover', 'PoiIssuingCountry', 'PlaceOfBirth',
        /// 'ComprehensiveIndex', 'IsIpMatched', 'ResolvedCountryCode'</param>
        public VerificationDetailsCollectionStepVariantDto(string name, VerificationDetailsProperty property)
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
        /// Gets or sets possible values include: 'IpAddress', 'TaxResidence',
        /// 'RiskLevel', 'IdDocumentNumber', 'Tin', 'Nationality', 'IsPep',
        /// 'IsSanctioned', 'IsAdverseMedia', 'Turnover', 'PoiIssuingCountry',
        /// 'PlaceOfBirth', 'ComprehensiveIndex', 'IsIpMatched',
        /// 'ResolvedCountryCode'
        /// </summary>
        [JsonProperty(PropertyName = "property")]
        public VerificationDetailsProperty Property { get; set; }

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
