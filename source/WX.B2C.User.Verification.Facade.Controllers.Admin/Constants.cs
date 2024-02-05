namespace WX.B2C.User.Verification.Facade.Controllers.Admin
{
    public static class Constants
    {
        public static class Headers
        {
            public const string CorrelationIdHeaderId = nameof(CorrelationIdHeaderId);
            public const string OperationIdHeaderId = nameof(OperationIdHeaderId);
        }

        public static class ErrorMessages
        {
            public const string ApplicationError = ""; // TODO: Fill up messages
            public const string ValidationError = "";
        }

        public static class ErrorCodes
        {
            public const string ApplicationError = "0000";
            public const string ValidationError = "0001";
            public const string ProfileNotFoundError = "0002";
            public const string BusinessError = "0003";
            public const string ApplicationNotFoundError = "0004";
            public const string CheckNotFoundError = "0005";
            public const string CollectionStepNotFoundError = "0006";
            public const string FileNotFoundError = "0007";
            public const string TaskNotFoundError = "0008";
            public const string DocumentNotFoundError = "0009";
            public const string CollectionStepReviewNotNeededError = "0010";
            public const string CheckVariantNotFound = "0011";
            public const string NoteNotFound = "0012";
            public const string CollectionStepInvalidState = "0013";
            public const string FileUploadedToOtherProvider = "0014";
        }
    }
}
