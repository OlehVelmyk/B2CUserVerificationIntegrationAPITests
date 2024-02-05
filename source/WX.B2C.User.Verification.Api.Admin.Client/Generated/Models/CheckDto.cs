// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CheckDto
    {
        /// <summary>
        /// Initializes a new instance of the CheckDto class.
        /// </summary>
        public CheckDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CheckDto class.
        /// </summary>
        /// <param name="type">Possible values include: 'IdentityDocument',
        /// 'IdentityEnhanced', 'FacialSimilarity', 'TaxResidence', 'IpMatch',
        /// 'FaceDuplication', 'NameAndDoBDuplication',
        /// 'IdDocNumberDuplication', 'FraudScreening', 'RiskListsScreening',
        /// 'Address', 'SurveyAnswers'</param>
        /// <param name="state">Possible values include: 'Pending', 'Running',
        /// 'Complete', 'Error', 'Cancelled'</param>
        /// <param name="result">Possible values include: 'Passed',
        /// 'Failed'</param>
        public CheckDto(System.Guid id, CheckType type, CheckVariantDto variant, CheckState state, IList<CollectionStepBriefDataDto> inputData, IList<DocumentDto> inputDocuments, IList<System.Guid> relatedTasks, CheckResult? result = default(CheckResult?), string decision = default(string), string outputData = default(string), IList<CheckErrorDto> errors = default(IList<CheckErrorDto>), System.DateTime? createdAt = default(System.DateTime?), System.DateTime? startedAt = default(System.DateTime?), System.DateTime? performedAt = default(System.DateTime?), System.DateTime? completedAt = default(System.DateTime?))
        {
            Id = id;
            Type = type;
            Variant = variant;
            State = state;
            Result = result;
            Decision = decision;
            InputData = inputData;
            InputDocuments = inputDocuments;
            OutputData = outputData;
            Errors = errors;
            RelatedTasks = relatedTasks;
            CreatedAt = createdAt;
            StartedAt = startedAt;
            PerformedAt = performedAt;
            CompletedAt = completedAt;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'IdentityDocument',
        /// 'IdentityEnhanced', 'FacialSimilarity', 'TaxResidence', 'IpMatch',
        /// 'FaceDuplication', 'NameAndDoBDuplication',
        /// 'IdDocNumberDuplication', 'FraudScreening', 'RiskListsScreening',
        /// 'Address', 'SurveyAnswers'
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public CheckType Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "variant")]
        public CheckVariantDto Variant { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Pending', 'Running',
        /// 'Complete', 'Error', 'Cancelled'
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public CheckState State { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Passed', 'Failed'
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public CheckResult? Result { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "decision")]
        public string Decision { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "inputData")]
        public IList<CollectionStepBriefDataDto> InputData { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "inputDocuments")]
        public IList<DocumentDto> InputDocuments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "outputData")]
        public string OutputData { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public IList<CheckErrorDto> Errors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "relatedTasks")]
        public IList<System.Guid> RelatedTasks { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdAt")]
        public System.DateTime? CreatedAt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "startedAt")]
        public System.DateTime? StartedAt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "performedAt")]
        public System.DateTime? PerformedAt { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "completedAt")]
        public System.DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Variant == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Variant");
            }
            if (InputData == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "InputData");
            }
            if (InputDocuments == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "InputDocuments");
            }
            if (RelatedTasks == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "RelatedTasks");
            }
            if (Variant != null)
            {
                Variant.Validate();
            }
            if (InputData != null)
            {
                foreach (var element in InputData)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (InputDocuments != null)
            {
                foreach (var element1 in InputDocuments)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
                    }
                }
            }
            if (Errors != null)
            {
                foreach (var element2 in Errors)
                {
                    if (element2 != null)
                    {
                        element2.Validate();
                    }
                }
            }
        }
    }
}