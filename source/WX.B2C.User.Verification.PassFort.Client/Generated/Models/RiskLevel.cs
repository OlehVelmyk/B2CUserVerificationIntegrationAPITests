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
    /// RiskLevel
    /// </summary>
    /// <remarks>
    /// Calculated risk level
    /// </remarks>
    public partial class RiskLevel
    {
        /// <summary>
        /// Initializes a new instance of the RiskLevel class.
        /// </summary>
        public RiskLevel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RiskLevel class.
        /// </summary>
        /// <param name="score">Calculated risk score</param>
        /// <param name="category">Possible values include: 'LOW', 'MEDIUM',
        /// 'HIGH', 'UNDETERMINED'</param>
        public RiskLevel(double? score = default(double?), string category = default(string))
        {
            Score = score;
            Category = category;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets calculated risk score
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double? Score { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'LOW', 'MEDIUM', 'HIGH',
        /// 'UNDETERMINED'
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

    }
}