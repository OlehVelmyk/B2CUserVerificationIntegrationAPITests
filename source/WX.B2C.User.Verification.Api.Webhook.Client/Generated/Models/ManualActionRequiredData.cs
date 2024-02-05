// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Webhook.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ManualActionRequiredData : Data
    {
        /// <summary>
        /// Initializes a new instance of the ManualActionRequiredData class.
        /// </summary>
        public ManualActionRequiredData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ManualActionRequiredData class.
        /// </summary>
        public ManualActionRequiredData(string profileId, IList<ActionDto> actions, string customerRef = default(string))
            : base(profileId, customerRef)
        {
            Actions = actions;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "actions")]
        public IList<ActionDto> Actions { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Actions == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Actions");
            }
            if (Actions != null)
            {
                foreach (var element in Actions)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
