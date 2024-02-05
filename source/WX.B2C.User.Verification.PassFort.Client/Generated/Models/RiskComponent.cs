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
    /// RiskComponent
    /// </summary>
    /// <remarks>
    /// Risk component for application
    /// </remarks>
    public partial class RiskComponent
    {
        /// <summary>
        /// Initializes a new instance of the RiskComponent class.
        /// </summary>
        public RiskComponent()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RiskComponent class.
        /// </summary>
        /// <param name="name">The name of this risk component</param>
        /// <param name="score">The contribution of this component to the
        /// overall risk score</param>
        /// <param name="required">Whether this component is required before an
        /// overall risk can be calculated</param>
        /// <param name="inputValue">The value used to calculate this risk
        /// category's score</param>
        /// <param name="rawValue">The raw input to this risk category</param>
        /// <param name="matchedRuleName">The name of the rule which matched
        /// the input value</param>
        public RiskComponent(string name, RiskCategory category = default(RiskCategory), string categoryId = default(string), double? score = default(double?), bool? required = default(bool?), string expiry = default(string), string inputValue = default(string), string rawValue = default(string), string matchedRuleName = default(string))
        {
            Name = name;
            Category = category;
            CategoryId = categoryId;
            Score = score;
            Required = required;
            Expiry = expiry;
            InputValue = inputValue;
            RawValue = rawValue;
            MatchedRuleName = matchedRuleName;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the name of this risk component
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public RiskCategory Category { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the contribution of this component to the overall risk
        /// score
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double? Score { get; set; }

        /// <summary>
        /// Gets or sets whether this component is required before an overall
        /// risk can be calculated
        /// </summary>
        [JsonProperty(PropertyName = "required")]
        public bool? Required { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public string Expiry { get; set; }

        /// <summary>
        /// Gets or sets the value used to calculate this risk category's score
        /// </summary>
        [JsonProperty(PropertyName = "input_value")]
        public string InputValue { get; set; }

        /// <summary>
        /// Gets or sets the raw input to this risk category
        /// </summary>
        [JsonProperty(PropertyName = "raw_value")]
        public string RawValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the rule which matched the input value
        /// </summary>
        [JsonProperty(PropertyName = "matched_rule_name")]
        public string MatchedRuleName { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Category != null)
            {
                Category.Validate();
            }
        }
    }
}
