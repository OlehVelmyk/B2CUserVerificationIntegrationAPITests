// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Public.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class UserActionDto
    {
        /// <summary>
        /// Initializes a new instance of the UserActionDto class.
        /// </summary>
        public UserActionDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the UserActionDto class.
        /// </summary>
        /// <param name="actionType">Possible values include: 'Survey',
        /// 'Selfie', 'TaxResidence', 'Tin', 'ProofOfIdentity',
        /// 'ProofOfAddress', 'ProofOfFunds', 'W9Form'</param>
        public UserActionDto(ActionType actionType, string reason, bool isOptional, int priority, IDictionary<string, string> actionData)
        {
            ActionType = actionType;
            Reason = reason;
            IsOptional = isOptional;
            Priority = priority;
            ActionData = actionData;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Survey', 'Selfie',
        /// 'TaxResidence', 'Tin', 'ProofOfIdentity', 'ProofOfAddress',
        /// 'ProofOfFunds', 'W9Form'
        /// </summary>
        [JsonProperty(PropertyName = "action_type")]
        public ActionType ActionType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "is_optional")]
        public bool IsOptional { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "action_data")]
        public IDictionary<string, string> ActionData { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Reason == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Reason");
            }
            if (ActionData == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ActionData");
            }
        }
    }
}
