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

    public partial class WebhookList
    {
        /// <summary>
        /// Initializes a new instance of the WebhookList class.
        /// </summary>
        public WebhookList()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the WebhookList class.
        /// </summary>
        public WebhookList(IList<Webhook> webhooks = default(IList<Webhook>))
        {
            Webhooks = webhooks;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "webhooks")]
        public IList<Webhook> Webhooks { get; set; }

    }
}