// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Risk result
    /// </summary>
    public partial class ProductApplicationRisk
    {
        /// <summary>
        /// Initializes a new instance of the ProductApplicationRisk class.
        /// </summary>
        public ProductApplicationRisk()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ProductApplicationRisk class.
        /// </summary>
        /// <param name="riskModel">Link to the risk model used to generate
        /// this risk result</param>
        /// <param name="isPending">True if the risk score is in the process of
        /// being recalculated</param>
        /// <param name="overall">Overall risk result</param>
        public ProductApplicationRisk(ProductApplicationRiskRiskModel riskModel = default(ProductApplicationRiskRiskModel), bool? isPending = default(bool?), ProductApplicationRiskOverall overall = default(ProductApplicationRiskOverall), string expiry = default(string), ProductApplicationRiskThresholds thresholds = default(ProductApplicationRiskThresholds))
        {
            RiskModel = riskModel;
            IsPending = isPending;
            Overall = overall;
            Expiry = expiry;
            Thresholds = thresholds;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets link to the risk model used to generate this risk
        /// result
        /// </summary>
        [JsonProperty(PropertyName = "risk_model")]
        public ProductApplicationRiskRiskModel RiskModel { get; set; }

        /// <summary>
        /// Gets or sets true if the risk score is in the process of being
        /// recalculated
        /// </summary>
        [JsonProperty(PropertyName = "is_pending")]
        public bool? IsPending { get; set; }

        /// <summary>
        /// Gets or sets overall risk result
        /// </summary>
        [JsonProperty(PropertyName = "overall")]
        public ProductApplicationRiskOverall Overall { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public string Expiry { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "thresholds")]
        public ProductApplicationRiskThresholds Thresholds { get; set; }

    }
}