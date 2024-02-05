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
    /// Associate
    /// </summary>
    /// <remarks>
    /// An associate to a PEP or Sanction
    /// </remarks>
    public partial class Associate
    {
        /// <summary>
        /// Initializes a new instance of the Associate class.
        /// </summary>
        public Associate()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Associate class.
        /// </summary>
        /// <param name="name">Name of the associate</param>
        /// <param name="association">Their association</param>
        /// <param name="isPep">Associate of PEP</param>
        /// <param name="wasPep">Was the associate of PEP</param>
        /// <param name="isSanction">Associate of Sanction</param>
        /// <param name="wasSanction">Was the associate of Sanction</param>
        /// <param name="deceasedDates">Reported death dates</param>
        /// <param name="inactiveAsRcaRelatedToPepDates">Dates of relation to
        /// PEP cessation</param>
        /// <param name="inactiveAsPepDates">Dates of PEP inactivity</param>
        /// <param name="dobs">Dates of Birth</param>
        public Associate(string name = default(string), string association = default(string), bool? isPep = default(bool?), bool? wasPep = default(bool?), bool? isSanction = default(bool?), bool? wasSanction = default(bool?), IList<string> deceasedDates = default(IList<string>), IList<string> inactiveAsRcaRelatedToPepDates = default(IList<string>), IList<string> inactiveAsPepDates = default(IList<string>), IList<string> dobs = default(IList<string>))
        {
            Name = name;
            Association = association;
            IsPep = isPep;
            WasPep = wasPep;
            IsSanction = isSanction;
            WasSanction = wasSanction;
            DeceasedDates = deceasedDates;
            InactiveAsRcaRelatedToPepDates = inactiveAsRcaRelatedToPepDates;
            InactiveAsPepDates = inactiveAsPepDates;
            Dobs = dobs;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of the associate
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets their association
        /// </summary>
        [JsonProperty(PropertyName = "association")]
        public string Association { get; set; }

        /// <summary>
        /// Gets or sets associate of PEP
        /// </summary>
        [JsonProperty(PropertyName = "is_pep")]
        public bool? IsPep { get; set; }

        /// <summary>
        /// Gets or sets was the associate of PEP
        /// </summary>
        [JsonProperty(PropertyName = "was_pep")]
        public bool? WasPep { get; set; }

        /// <summary>
        /// Gets or sets associate of Sanction
        /// </summary>
        [JsonProperty(PropertyName = "is_sanction")]
        public bool? IsSanction { get; set; }

        /// <summary>
        /// Gets or sets was the associate of Sanction
        /// </summary>
        [JsonProperty(PropertyName = "was_sanction")]
        public bool? WasSanction { get; set; }

        /// <summary>
        /// Gets or sets reported death dates
        /// </summary>
        [JsonProperty(PropertyName = "deceased_dates")]
        public IList<string> DeceasedDates { get; set; }

        /// <summary>
        /// Gets or sets dates of relation to PEP cessation
        /// </summary>
        [JsonProperty(PropertyName = "inactive_as_rca_related_to_pep_dates")]
        public IList<string> InactiveAsRcaRelatedToPepDates { get; set; }

        /// <summary>
        /// Gets or sets dates of PEP inactivity
        /// </summary>
        [JsonProperty(PropertyName = "inactive_as_pep_dates")]
        public IList<string> InactiveAsPepDates { get; set; }

        /// <summary>
        /// Gets or sets dates of Birth
        /// </summary>
        [JsonProperty(PropertyName = "dobs")]
        public IList<string> Dobs { get; set; }

    }
}
