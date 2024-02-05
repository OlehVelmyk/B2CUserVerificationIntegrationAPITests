namespace WX.B2C.User.Verification.Provider.Contracts
{
    public static class Constants
    {
        public static class ErrorCodes
        {
            public const string MissingRequiredFields = "101";
            public const string ConfigurationError = "102";

            public const string MissingApiKey = "201";
            public const string InvalidApiKey = "202";
            public const string InvalidInputData = "203";

            public const string ProviderConnectionError = "301";
            public const string ProviderUnknownError = "302";
            public const string ProviderInvalidResponse = "303";

            public const string UnknownInternalError = "401";
            public const string ProcessingError = "402";

            public const string UnsupportedFunctionality = "501";
        }
    }
}
