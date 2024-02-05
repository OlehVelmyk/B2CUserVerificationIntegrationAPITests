// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class WorkflowStatus
    {
        /// <summary>
        /// Initializes a new instance of the WorkflowStatus class.
        /// </summary>
        public WorkflowStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the WorkflowStatus class.
        /// </summary>
        /// <param name="conversationId">Unique identifier that is assigned to
        /// the transaction by RDP.</param>
        /// <param name="requestId">Unique identifier that is assigned to the
        /// transaction request by RDP.</param>
        /// <param name="transactionStatus">Current status of the transaction,
        /// the transaction number, and the result of the transaction.</param>
        /// <param name="reference">Internal tracking number that your
        /// organization can assign to a transaction.</param>
        public WorkflowStatus(string conversationId = default(string), string requestId = default(string), string transactionStatus = default(string), string reference = default(string))
        {
            ConversationId = conversationId;
            RequestId = requestId;
            TransactionStatus = transactionStatus;
            Reference = reference;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique identifier that is assigned to the transaction
        /// by RDP.
        /// </summary>
        [JsonProperty(PropertyName = "ConversationId")]
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets unique identifier that is assigned to the transaction
        /// request by RDP.
        /// </summary>
        [JsonProperty(PropertyName = "RequestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets current status of the transaction, the transaction
        /// number, and the result of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "TransactionStatus")]
        public string TransactionStatus { get; set; }

        /// <summary>
        /// Gets or sets internal tracking number that your organization can
        /// assign to a transaction.
        /// </summary>
        [JsonProperty(PropertyName = "Reference")]
        public string Reference { get; set; }

    }
}