namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Constants
{
    internal static class Parameters
    {
        public const string AnyGuidPattern = "[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}";

        public const string CheckIdTemplate = "{checkIdTemplate}";
        public const string CheckResult = "{result}";
        public const string CheckSubResult = "{subResult}";
        public const string ReportResult = "{result}";
        public const string ReportSubResult = "{subResult}";
        public const string FontsResult = "{fontsResult}";
        public const string ImageIntegrityResult = "{imageIntegrity}";
        public const string PoiIssuingCountry = "{poiIssuingCountry}";
        public const string FaceComparison = "{faceComparison}";
        public const string PreviouslySeenFaces = "{previouslySeenFaces}";
        public const string ReportName = "{reportName}";
        public const string DocumentId = "{documentId}";
        public const string PhotoId = "{photoId}";
        public const string VideoId = "{videoId}";
        public const string ContentType = "{contentType}";
        public const string WebhookUrl = "{webhookUrl}";
        public const string WebhookBody = "{webhookBody}";
    }
}
