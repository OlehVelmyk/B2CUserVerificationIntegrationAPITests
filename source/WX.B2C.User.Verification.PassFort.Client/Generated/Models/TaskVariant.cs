// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// TaskVariant
    /// </summary>
    /// <remarks>
    /// The identifier of a variant of a task.
    /// </remarks>
    public partial class TaskVariant
    {
        /// <summary>
        /// Initializes a new instance of the TaskVariant class.
        /// </summary>
        public TaskVariant()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TaskVariant class.
        /// </summary>
        /// <param name="alias">Alias of this task variant</param>
        /// <param name="name">Name of the task variant</param>
        /// <param name="description">Description of the task variant</param>
        /// <param name="taskType">Possible values include:
        /// 'INDIVIDUAL_VERIFY_IDENTITY', 'INDIVIDUAL_VERIFY_ADDRESS',
        /// 'INDIVIDUAL_VERIFY_SOURCE_OF_FUNDS',
        /// 'INDIVIDUAL_ASSESS_MEDIA_AND_POLITICAL_AND_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_POLITICAL_AND_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_POLITICAL_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_VERIFY_BANK_ACCOUNT',
        /// 'INDIVIDUAL_VERIFY_IMMIGRATION_STATUS', 'INDIVIDUAL_MANUAL_TASK',
        /// 'INDIVIDUAL_ASSESS_DEVICE_REPUTATION',
        /// 'INDIVIDUAL_FRAUD_SCREENING', 'INDIVIDUAL_VERIFY_TAX_STATUS',
        /// 'COMPANY_VERIFY_IDENTITY', 'COMPANY_VERIFY_ADDRESS',
        /// 'COMPANY_VERIFY_CHARITY', 'COMPANY_IDENTIFY_AUTHORIZED_PERSONS',
        /// 'COMPANY_IDENTIFY_OFFICERS', 'COMPANY_IDENTIFY_TRUSTEES',
        /// 'COMPANY_IDENTIFY_BENEFICIAL_OWNERS', 'COMPANY_REVIEW_FILINGS',
        /// 'COMPANY_ASSESS_SANCTIONS_EXPOSURE',
        /// 'COMPANY_ASSESS_MEDIA_EXPOSURE',
        /// 'COMPANY_ASSESS_MEDIA_AND_SANCTIONS_EXPOSURE',
        /// 'COMPANY_MANUAL_TASK', 'COMPANY_VERIFY_BANK_ACCOUNT',
        /// 'COMPANY_VERIFY_TAX_STATUS', 'COMPANY_ASSESS_FINANCIALS',
        /// 'COMPANY_FRAUD_SCREENING',
        /// 'COMPANY_MERCHANT_FRAUD_SCREENING'</param>
        public TaskVariant(string id, string alias = default(string), string name = default(string), string description = default(string), TaskType? taskType = default(TaskType?))
        {
            Id = id;
            Alias = alias;
            Name = name;
            Description = description;
            TaskType = taskType;
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
        /// Gets or sets alias of this task variant
        /// </summary>
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets name of the task variant
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description of the task variant
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'INDIVIDUAL_VERIFY_IDENTITY',
        /// 'INDIVIDUAL_VERIFY_ADDRESS', 'INDIVIDUAL_VERIFY_SOURCE_OF_FUNDS',
        /// 'INDIVIDUAL_ASSESS_MEDIA_AND_POLITICAL_AND_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_POLITICAL_AND_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_POLITICAL_EXPOSURE',
        /// 'INDIVIDUAL_ASSESS_SANCTIONS_EXPOSURE',
        /// 'INDIVIDUAL_VERIFY_BANK_ACCOUNT',
        /// 'INDIVIDUAL_VERIFY_IMMIGRATION_STATUS', 'INDIVIDUAL_MANUAL_TASK',
        /// 'INDIVIDUAL_ASSESS_DEVICE_REPUTATION',
        /// 'INDIVIDUAL_FRAUD_SCREENING', 'INDIVIDUAL_VERIFY_TAX_STATUS',
        /// 'COMPANY_VERIFY_IDENTITY', 'COMPANY_VERIFY_ADDRESS',
        /// 'COMPANY_VERIFY_CHARITY', 'COMPANY_IDENTIFY_AUTHORIZED_PERSONS',
        /// 'COMPANY_IDENTIFY_OFFICERS', 'COMPANY_IDENTIFY_TRUSTEES',
        /// 'COMPANY_IDENTIFY_BENEFICIAL_OWNERS', 'COMPANY_REVIEW_FILINGS',
        /// 'COMPANY_ASSESS_SANCTIONS_EXPOSURE',
        /// 'COMPANY_ASSESS_MEDIA_EXPOSURE',
        /// 'COMPANY_ASSESS_MEDIA_AND_SANCTIONS_EXPOSURE',
        /// 'COMPANY_MANUAL_TASK', 'COMPANY_VERIFY_BANK_ACCOUNT',
        /// 'COMPANY_VERIFY_TAX_STATUS', 'COMPANY_ASSESS_FINANCIALS',
        /// 'COMPANY_FRAUD_SCREENING', 'COMPANY_MERCHANT_FRAUD_SCREENING'
        /// </summary>
        [JsonProperty(PropertyName = "task_type")]
        public TaskType? TaskType { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Id");
            }
        }
    }
}
