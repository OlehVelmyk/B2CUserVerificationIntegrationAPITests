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
    /// CheckResource
    /// </summary>
    /// <remarks>
    /// A single check run against a profile
    /// </remarks>
    [Newtonsoft.Json.JsonObject("CheckResource")]
    public partial class CheckResource
    {
        /// <summary>
        /// Initializes a new instance of the CheckResource class.
        /// </summary>
        public CheckResource()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CheckResource class.
        /// </summary>
        /// <param name="state">Possible values include: 'PENDING', 'RUNNING',
        /// 'COMPLETE'</param>
        /// <param name="errors">An array of errors that occurred when the
        /// check was performed. Hopefully empty!</param>
        /// <param name="taskIds">The tasks related to this check</param>
        /// <param name="instructedExternally">Whether the check ran externally
        /// and contains the formatted result</param>
        /// <param name="decision">Possible values include: 'PASS', 'FAIL',
        /// 'PARTIAL', 'WARN', 'ERROR'</param>
        /// <param name="providers">An array of records on the providers that
        /// were used to run this check</param>
        public CheckResource(string id = default(string), CheckVariant variant = default(CheckVariant), CheckState? state = default(CheckState?), string performedOn = default(string), IList<Error> errors = default(IList<Error>), IList<string> taskIds = default(IList<string>), bool? instructedExternally = default(bool?), DecisionClass? decision = default(DecisionClass?), IList<CheckResourceProvidersItem> providers = default(IList<CheckResourceProvidersItem>))
        {
            Id = id;
            Variant = variant;
            State = state;
            PerformedOn = performedOn;
            Errors = errors;
            TaskIds = taskIds;
            InstructedExternally = instructedExternally;
            Decision = decision;
            Providers = providers;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "variant")]
        public CheckVariant Variant { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'PENDING', 'RUNNING',
        /// 'COMPLETE'
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public CheckState? State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "performed_on")]
        public string PerformedOn { get; set; }

        /// <summary>
        /// Gets or sets an array of errors that occurred when the check was
        /// performed. Hopefully empty!
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public IList<Error> Errors { get; set; }

        /// <summary>
        /// Gets or sets the tasks related to this check
        /// </summary>
        [JsonProperty(PropertyName = "task_ids")]
        public IList<string> TaskIds { get; set; }

        /// <summary>
        /// Gets or sets whether the check ran externally and contains the
        /// formatted result
        /// </summary>
        [JsonProperty(PropertyName = "instructed_externally")]
        public bool? InstructedExternally { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'PASS', 'FAIL', 'PARTIAL',
        /// 'WARN', 'ERROR'
        /// </summary>
        [JsonProperty(PropertyName = "decision")]
        public DecisionClass? Decision { get; set; }

        /// <summary>
        /// Gets or sets an array of records on the providers that were used to
        /// run this check
        /// </summary>
        [JsonProperty(PropertyName = "providers")]
        public IList<CheckResourceProvidersItem> Providers { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Variant != null)
            {
                Variant.Validate();
            }
        }
    }
}
