namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Constants
{
    internal static class DecorateFunctions
    {
        public const string Folder = "Mountebank/DecorateFunctions";

        // Onfido
        public static readonly string OnfidoDocumentReportDecorator = $"{nameof(OnfidoDocumentReportDecorator)}.js";
        public static readonly string OnfidoCreateCheckDecorator = $"{nameof(OnfidoCreateCheckDecorator)}.js";
        public static readonly string OnfidoGetCheckDecorator = $"{nameof(OnfidoGetCheckDecorator)}.js";
        public static readonly string OnfidoUploadDocumentDecorator = $"{nameof(OnfidoUploadDocumentDecorator)}.js";
        public static readonly string OnfidoUploadLivePhotoDecorator = $"{nameof(OnfidoUploadLivePhotoDecorator)}.js";

        // PassFort
        public static readonly string PassfortRunCheckDecorator = $"{nameof(PassfortRunCheckDecorator)}.js";
    }
}
