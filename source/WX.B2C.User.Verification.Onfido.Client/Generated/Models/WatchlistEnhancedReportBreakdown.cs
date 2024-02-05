// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class WatchlistEnhancedReportBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the WatchlistEnhancedReportBreakdown
        /// class.
        /// </summary>
        public WatchlistEnhancedReportBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the WatchlistEnhancedReportBreakdown
        /// class.
        /// </summary>
        public WatchlistEnhancedReportBreakdown(BreakdownResult politicallyExposedPerson = default(BreakdownResult), BreakdownResult sanction = default(BreakdownResult), BreakdownResult adverseMedia = default(BreakdownResult), BreakdownResult monitoredLists = default(BreakdownResult))
        {
            PoliticallyExposedPerson = politicallyExposedPerson;
            Sanction = sanction;
            AdverseMedia = adverseMedia;
            MonitoredLists = monitoredLists;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "politically_exposed_person")]
        public BreakdownResult PoliticallyExposedPerson { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sanction")]
        public BreakdownResult Sanction { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adverse_media")]
        public BreakdownResult AdverseMedia { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "monitored_lists")]
        public BreakdownResult MonitoredLists { get; set; }

    }
}