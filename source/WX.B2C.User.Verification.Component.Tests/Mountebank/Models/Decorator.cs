using MbDotNet.Models.Responses;
using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Models
{
    internal class Decorator : Behavior
    {
        [JsonProperty("decorate")]
        public string DecorateFunction { get; set; }
    }
}
