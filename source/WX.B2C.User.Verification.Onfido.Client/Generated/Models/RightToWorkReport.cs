// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [Newtonsoft.Json.JsonObject("right_to_work")]
    public partial class RightToWorkReport : Report
    {
        /// <summary>
        /// Initializes a new instance of the RightToWorkReport class.
        /// </summary>
        public RightToWorkReport()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RightToWorkReport class.
        /// </summary>
        /// <param name="id">The unique identifier for the report.
        /// Read-only.</param>
        /// <param name="createdAt">The date and time at which the report was
        /// first initiated. Read-only.</param>
        /// <param name="href">The API endpoint to retrieve the report.
        /// Read-only.</param>
        /// <param name="status">The current state of the report in the
        /// checking process. Read-only.</param>
        /// <param name="result">The result of the report. Read-only. Possible
        /// values include: 'clear', 'consider', 'unidentified'</param>
        /// <param name="subResult">The sub_result of the report. It gives a
        /// more detailed result for document reports only, and will be null
        /// otherwise. Read-only. Possible values include: 'clear', 'rejected',
        /// 'suspected', 'caution'</param>
        /// <param name="checkId">The ID of the check to which the report
        /// belongs. Read-only.</param>
        /// <param name="documents">Array of objects with document ids that
        /// were used in the Onfido engine. [ONLY POPULATED FOR DOCUMENT AND
        /// FACIAL SIMILARITY REPORTS]</param>
        public RightToWorkReport(string id, System.DateTime? createdAt = default(System.DateTime?), string href = default(string), string status = default(string), ReportResult? result = default(ReportResult?), ReportSubResult? subResult = default(ReportSubResult?), string checkId = default(string), IList<DocumentItem> documents = default(IList<DocumentItem>), RightToWorkReportBreakdown breakdown = default(RightToWorkReportBreakdown), RightToWorkReportProperties properties = default(RightToWorkReportProperties))
            : base(id, createdAt, href, status, result, subResult, checkId, documents)
        {
            Breakdown = breakdown;
            Properties = properties;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "breakdown")]
        public RightToWorkReportBreakdown Breakdown { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public RightToWorkReportProperties Properties { get; set; }

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
