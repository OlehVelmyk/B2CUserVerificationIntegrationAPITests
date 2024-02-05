using System.IO;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Constants
{
    internal static class Templates
    {
        public const string Folder = "Mountebank/Templates";

        // B2C.Survey
        public static readonly string UserSurveyTemplate = $"{nameof(UserSurveyTemplate)}.json";
        public static readonly string TaggedAnswersTemplate = $"{nameof(TaggedAnswersTemplate)}.json";

        // Onfido
        public static readonly string OnfidoWebhookRequest = $"{nameof(OnfidoWebhookRequest)}.json";
        public static readonly string OnfidoCheckResponse = $"{nameof(OnfidoCheckResponse)}.json";
        public static readonly string OnfidoFaceDuplicationReport = $"{nameof(OnfidoFaceDuplicationReport)}.json";
        public static readonly string OnfidoFacialSimilarityReport = $"{nameof(OnfidoFacialSimilarityReport)}.json";
        public static readonly string OnfidoIdentityDocumentReport = $"{nameof(OnfidoIdentityDocumentReport)}.json";
        public static readonly string OnfidoIdentityEnhancedReport = $"{nameof(OnfidoIdentityEnhancedReport)}.json";
        public static readonly string OnfidoDocumentResponse = $"{nameof(OnfidoDocumentResponse)}.json";
        public static readonly string OnfidoLivePhotoResponse = $"{nameof(OnfidoLivePhotoResponse)}.json";
        public static readonly string OnfidoLiveVideoResponse = $"{nameof(OnfidoLiveVideoResponse)}.json";

        // PassFort
        public static readonly string PassfortWebhookRequest = $"{nameof(PassfortWebhookRequest)}.json";
        public static readonly string PassfortCheckResponse = $"{nameof(PassfortCheckResponse)}.json";
        public static string PassfortUpdateProfileResponse = $"{nameof(PassfortUpdateProfileResponse)}.json";
    }
}
