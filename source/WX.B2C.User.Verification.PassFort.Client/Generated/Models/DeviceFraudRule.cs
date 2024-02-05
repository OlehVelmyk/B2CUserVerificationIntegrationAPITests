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
    /// DeviceFraudRule
    /// </summary>
    /// <remarks>
    /// Matching fraud rule for a device
    /// </remarks>
    public partial class DeviceFraudRule
    {
        /// <summary>
        /// Initializes a new instance of the DeviceFraudRule class.
        /// </summary>
        public DeviceFraudRule()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DeviceFraudRule class.
        /// </summary>
        /// <param name="name">Name of the rule</param>
        /// <param name="reason">Reason this rule was activated</param>
        /// <param name="score">Contribution to the score from this
        /// rule</param>
        public DeviceFraudRule(string name = default(string), string reason = default(string), int? score = default(int?))
        {
            Name = name;
            Reason = reason;
            Score = score;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of the rule
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets reason this rule was activated
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets contribution to the score from this rule
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public int? Score { get; set; }

    }
}
