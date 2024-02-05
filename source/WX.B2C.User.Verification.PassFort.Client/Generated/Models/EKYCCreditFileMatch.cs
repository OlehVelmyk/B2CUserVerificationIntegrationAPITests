// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// EKYCCreditFileMatch
    /// </summary>
    /// <remarks>
    /// Describes a credit file match (if there was one)
    /// </remarks>
    public partial class EKYCCreditFileMatch
    {
        /// <summary>
        /// Initializes a new instance of the EKYCCreditFileMatch class.
        /// </summary>
        public EKYCCreditFileMatch()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EKYCCreditFileMatch class.
        /// </summary>
        /// <param name="uniqueNumber">The unique reference for this credit
        /// file. Will be null if there was no hit</param>
        /// <param name="bureauName">The name of the bureau owning this
        /// file</param>
        /// <param name="extra">List of other fields that were retrieved from
        /// the bureau</param>
        public EKYCCreditFileMatch(string uniqueNumber = default(string), string bureauName = default(string), IList<EKYCRecordField> extra = default(IList<EKYCRecordField>))
        {
            UniqueNumber = uniqueNumber;
            BureauName = bureauName;
            Extra = extra;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the unique reference for this credit file. Will be
        /// null if there was no hit
        /// </summary>
        [JsonProperty(PropertyName = "unique_number")]
        public string UniqueNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the bureau owning this file
        /// </summary>
        [JsonProperty(PropertyName = "bureau_name")]
        public string BureauName { get; set; }

        /// <summary>
        /// Gets or sets list of other fields that were retrieved from the
        /// bureau
        /// </summary>
        [JsonProperty(PropertyName = "extra")]
        public IList<EKYCRecordField> Extra { get; set; }

    }
}
