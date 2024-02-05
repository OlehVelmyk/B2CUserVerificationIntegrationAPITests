using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Api.Webhook.Client.Models
{
    public partial class PassFortEventRequest
    {
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }
    }
}
