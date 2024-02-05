// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ReviewCollectionStepRequest
    {
        /// <summary>
        /// Initializes a new instance of the ReviewCollectionStepRequest
        /// class.
        /// </summary>
        public ReviewCollectionStepRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ReviewCollectionStepRequest
        /// class.
        /// </summary>
        /// <param name="reviewResult">Possible values include: 'Approved',
        /// 'Rejected'</param>
        public ReviewCollectionStepRequest(CollectionStepReviewResult reviewResult, string reason)
        {
            ReviewResult = reviewResult;
            Reason = reason;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Approved', 'Rejected'
        /// </summary>
        [JsonProperty(PropertyName = "reviewResult")]
        public CollectionStepReviewResult ReviewResult { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

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
        }
    }
}
