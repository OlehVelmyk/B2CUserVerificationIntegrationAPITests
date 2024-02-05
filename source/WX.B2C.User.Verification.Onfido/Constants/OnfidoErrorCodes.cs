namespace WX.B2C.User.Verification.Onfido.Constants
{
    internal static class DocumentReportErrorCodes
    {
        public const string InvalidReport = "0000";
        public const string NotClearReportResult = "1000";

        // 10xx - document report validation errors
        public const string CautionImageIntegrityColourPicture = "1001";
        public const string CautionImageIntegrityConclusiveDocumentQuality = "1002";
        public const string CautionImageIntegritySupportedDocument = "1003";
        public const string CautionImageIntegrityImageQuality = "1004";

        // 11xx - data comparison breakdown validation errors
        public const string CautionDataComparisonDocumentType = "1101";
        public const string CautionDataComparisonDateOfBirth = "1102";
        public const string CautionDataComparisonLastName = "1103";
        public const string CautionDataComparisonFirstName = "1104";

        // 12xx - data validation breakdown validation errors
        public const string SuspectedDataValidationMrz = "1201";
        public const string SuspectedDataValidationDocumentExpiration = "1202";
        public const string SuspectedDataValidationExpiryDate = "1203";
        public const string SuspectedDataValidationDateOfBirth = "1204";

        // 13xx - visual authentication breakdown validation errors
        public const string SuspectedVisualAuthenticationOriginalDocumentPresent = "1301";
        public const string SuspectedVisualAuthenticationDigitalTampering = "1302";
        public const string SuspectedVisualAuthenticationPictureFaceIntegrity = "1303";
        public const string SuspectedVisualAuthenticationSecurityFeatures = "1304";
        public const string SuspectedVisualAuthenticationTemplate = "1305";
        public const string SuspectedVisualAuthenticationFonts = "1306";
        public const string SuspectedVisualAuthenticationFaceDetection = "1307";
        public const string SuspectedVisualAuthenticationOther = "1308";

        // 14xx - data consistency breakdown validation errors
        public const string SuspectedDataConsistencyDocumentType = "1401";
        public const string SuspectedDataConsistencyDateOfBirth = "1402";
        public const string SuspectedDataConsistencyDateOfExpiry = "1403";
        public const string SuspectedDataConsistencyLastName = "1404";
        public const string SuspectedDataConsistencyFirstName = "1405";

        // 15xx - compromised documents breakdown validation errors
        public const string SuspectedCompromisedDocumentsDocumentNotFound = "1501";
    }

    internal static class FacialSimilarityReportErrorCodes
    {
        public const string InvalidReport = "0000";

        // 10xx - visual authenticity breakdown validation errors
        public const string CautionVisualAuthenticity = "1001";

        // 11xx - image integrity breakdown validation errors
        public const string CautionImageIntegrity = "1101";

        // 12xx - face comparison breakdown validation errors
        public const string CautionFaceMatchesFound = "1201";
    }

    internal static class KnownFacesReportErrorCodes
    {
        public const string InvalidReport = "0000";
        public const string NotClearReportResult = "1000";
        public const string CautionImageIntegrity = "1001";
        public const string CautionPreviouslySeenFaces = "1002";
    }
}
