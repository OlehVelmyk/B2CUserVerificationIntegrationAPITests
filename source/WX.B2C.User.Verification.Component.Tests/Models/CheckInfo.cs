using System;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal sealed class CheckInfo
    {
        [JsonProperty]
        public Guid VariantId { get; private set; }

        [JsonProperty]
        public CheckType Type { get; private set; }

        [JsonProperty]
        public CheckProviderType Provider { get; private set; }

        [JsonProperty]
        public CollectionStepVariantDto[] RequiredData { get; private set; }
    }
}
