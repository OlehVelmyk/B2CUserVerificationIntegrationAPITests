// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class WatchlistReportBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the WatchlistReportBreakdown class.
        /// </summary>
        public WatchlistReportBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the WatchlistReportBreakdown class.
        /// </summary>
        public WatchlistReportBreakdown(BreakdownResult sanction = default(BreakdownResult), BreakdownResult politicallyExposedPerson = default(BreakdownResult), BreakdownResult legalAndRegulatoryWarnings = default(BreakdownResult))
        {
            Sanction = sanction;
            PoliticallyExposedPerson = politicallyExposedPerson;
            LegalAndRegulatoryWarnings = legalAndRegulatoryWarnings;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sanction")]
        public BreakdownResult Sanction { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "politically_exposed_person")]
        public BreakdownResult PoliticallyExposedPerson { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "legal_and_regulatory_warnings")]
        public BreakdownResult LegalAndRegulatoryWarnings { get; set; }

    }
}