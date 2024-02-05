// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Asserts whether the applicant has the right to work.
    /// </summary>
    public partial class RightToWorkRightToWork : BreakdownResult
    {
        /// <summary>
        /// Initializes a new instance of the RightToWorkRightToWork class.
        /// </summary>
        public RightToWorkRightToWork()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RightToWorkRightToWork class.
        /// </summary>
        public RightToWorkRightToWork(string result = default(string), RightToWorkRightToWorkBreakdown breakdown = default(RightToWorkRightToWorkBreakdown))
            : base(result)
        {
            Breakdown = breakdown;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "breakdown")]
        public RightToWorkRightToWorkBreakdown Breakdown { get; set; }

    }
}
