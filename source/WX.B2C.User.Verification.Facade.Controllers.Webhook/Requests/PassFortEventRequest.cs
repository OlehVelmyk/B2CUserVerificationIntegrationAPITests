using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests
{
    [KnownType(typeof(PassFortCheckCompletedEventRequest))]
    [KnownType(typeof(PassFortProductStatusChangedEventRequest))]
    [KnownType(typeof(PassFortManualActionRequiredEventRequest))]
    [KnownType(typeof(UnsupportedPassFortEventRequest))]
    [KnownType(typeof(PassFortEventRequest))]
    public abstract class PassFortEventRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }

    public class PassFortCheckCompletedEventRequest : PassFortEventRequest
    {
        [JsonPropertyName("data")]
        public CheckCompletedData Data { get; set; }
    }

    public class PassFortProductStatusChangedEventRequest : PassFortEventRequest
    {
        [JsonPropertyName("data")]
        public ProductStatusChangedData Data { get; set; }
    }

    public class PassFortManualActionRequiredEventRequest : PassFortEventRequest
    {
        [JsonPropertyName("data")]
        public ManualActionRequiredData Data { get; set; }
    }

    public class UnsupportedPassFortEventRequest : PassFortEventRequest
    {
    }

    [KnownType(typeof(CheckCompletedData))]
    [KnownType(typeof(ProductStatusChangedData))]
    [KnownType(typeof(ManualActionRequiredData))]
    [KnownType(typeof(Data))]
    public class Data
    {
        [NotRequired]
        [JsonPropertyName("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonPropertyName("profile_id")]
        public string ProfileId { get; set; }
    }

    public class CheckCompletedData : Data
    {
        [JsonPropertyName("check")]
        public CheckDto Check { get; set; }
    }

    public class ProductStatusChangedData : Data
    {
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; }

        [JsonPropertyName("new_status")]
        public string NewStatus { get; set; }

        [JsonPropertyName("product")]
        public ProductDto Product { get; set; }
    }

    public class ManualActionRequiredData : Data
    {
        [JsonPropertyName("actions")]
        public ActionDto[] Actions { get; set; }
    }

    public class CheckDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("check_type")]
        public string CheckType { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }
    }

    public class ProductDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }
    }

    public class VariantDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [NotRequired]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }
    }

    public class ActionDto
    {
        [JsonPropertyName("action_type")]
        public string ActionType { get; set; }

        [JsonPropertyName("applications")]
        public ApplicationDto[] Applications { get; set; }

        [JsonPropertyName("due")]
        public string Due { get; set; }

        [JsonPropertyName("task")]
        public TaskDto Task { get; set; }
    }

    public class ApplicationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("product")]
        public ProductDto Product { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class TaskDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("variant")]
        public VariantDto Variant { get; set; }
    }
}
