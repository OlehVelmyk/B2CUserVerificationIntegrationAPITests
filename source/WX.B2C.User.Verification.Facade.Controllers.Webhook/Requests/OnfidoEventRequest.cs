using System.Text.Json.Serialization;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests
{
    public class OnfidoEventRequest
    {
        [JsonPropertyName("payload")]
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        [JsonPropertyName("resource_type")]
        public string ResourceType { get; set; }
        
        [JsonPropertyName("action")]
        public string Action { get; set; }
        
        [JsonPropertyName("object")]
        public Object Object { get; set; }
    }

    public class Object
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("completed_at")]
        public string CompletedAt { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    public class ResourceTypes
    {
        public const string Check = "check";
        public const string Report = "report";
    }

    public class CheckStatus
    {
        public const string InProgress = "in_progress";
        public const string AwaitingApplicant = "awaiting_applicant";
        public const string Withdrawn = "withdrawn";
        public const string Complete = "complete";
        public const string Paused = "paused";
        public const string Reopened = "reopened";
    }

    public class ReportStatus
    {
        public const string AwaitingData = "awaiting_data";
        public const string AwaitingApproval = "awaiting_approval";
        public const string Cancelled = "cancelled";
        public const string Complete = "complete";
        public const string Withdrawn = "withdrawn";
        public const string Paused = "paused";
    }
}
