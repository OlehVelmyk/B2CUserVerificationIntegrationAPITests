using System.Collections.Generic;

namespace WX.B2C.User.Verification.Facade.Controllers.Public
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
            public const string ApplicationError = "Sorry, something went wrong. Please try again later or contact our support team for assistance.";
            public const string ActionIsNotAllowed = "Action is not allowed";
        }

        public static class ErrorCodes
        {
            public const string ApplicationError = nameof(ApplicationError);
            public const string ValidationError = nameof(ValidationError);
            public const string ProfileNotFoundError = nameof(ProfileNotFoundError);
            public const string UserAddressNotFoundError = nameof(UserAddressNotFoundError);
            public const string BusinessError = nameof(BusinessError);
            public const string ValidationRulesNotFoundError = nameof(ValidationRulesNotFoundError);
            public const string ApplicationNotAvailableError = nameof(ApplicationNotAvailableError);
            public const string ApplicationAlreadyCreatedError = nameof(ApplicationAlreadyCreatedError);
            public const string ActionIsNotAllowed = nameof(ActionIsNotAllowed);
            public const string DocumentTypeIsNotAllowed = nameof(DocumentTypeIsNotAllowed);
            public const string FileTypeNotSupported = nameof(FileTypeNotSupported);
            public const string FileSizeTooLarge = nameof(FileSizeTooLarge);
            public const string FileCanNotBeEmpty = nameof(FileCanNotBeEmpty);
            public const string IncorrectFilesQuantity = nameof(IncorrectFilesQuantity);
            public const string ExternalFileNotFound = nameof(ExternalFileNotFound);

            public static string[] Defined => new[]
            {
                ApplicationError, 
                ValidationError, 
                ProfileNotFoundError, 
                UserAddressNotFoundError, 
                BusinessError,
                ValidationRulesNotFoundError, 
                ApplicationNotAvailableError, 
                ApplicationAlreadyCreatedError, 
                ActionIsNotAllowed,
                DocumentTypeIsNotAllowed,
                FileTypeNotSupported,
                FileSizeTooLarge,
                FileCanNotBeEmpty,
                IncorrectFilesQuantity,
                ExternalFileNotFound
            };
        }

        public static class ClaimTypes
        {
            public const string OwnerId = nameof(OwnerId);
            public const string ClientIp = nameof(ClientIp);
        }
    }
}
