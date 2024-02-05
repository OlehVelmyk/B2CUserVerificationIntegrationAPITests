using System.IO;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class Global
    {
        public static readonly string RootPath = Directory.GetCurrentDirectory();

        public const string SettingsFileName = "testSettings.json";

        public const string PublicApiEndpoint = nameof(PublicApiEndpoint);
        public const string AdminApiEndpoint = nameof(AdminApiEndpoint);
        public const string InternalApiEndpoint = nameof(InternalApiEndpoint);
        public const string WebhookApiEndpoint = nameof(WebhookApiEndpoint);
        public const string B2cSurveyApiEndpoint = nameof(B2cSurveyApiEndpoint);
        public const string ConnectionString = nameof(ConnectionString);
        public const string OnfidoApiUrl = nameof(OnfidoApiUrl);
        public const string OnfidoApiToken = nameof(OnfidoApiToken);
        public const string PassFortApiUrl = nameof(PassFortApiUrl);
        public const string LocalOnfidoApiUrl = nameof(LocalOnfidoApiUrl);
        public const string LocalPassFortApiUrl = nameof(LocalPassFortApiUrl);
    }
}
