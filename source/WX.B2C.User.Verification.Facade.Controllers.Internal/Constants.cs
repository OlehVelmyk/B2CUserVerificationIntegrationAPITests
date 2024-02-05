namespace WX.B2C.User.Verification.Facade.Controllers.Internal
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
            public const string ApplicationError = "";
            public const string ValidationError = "";
        }

        public static class ErrorCodes
        {
            public const string ApplicationError = "0000";
            public const string ValidationError = "0001";
            public const string ProfileNotFoundError = "0002";
            public const string BusinessError = "0003";
            public const string ApplicationNotFoundError = "0004";
            public const string DocumentNotFoundError = "0005";
        }
    }
}