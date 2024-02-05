using FluentValidation;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class UploadDocumentFileRequestValidator : BaseRequestValidator<UploadDocumentFileRequest>
    {
        public UploadDocumentFileRequestValidator()
        {
            RuleFor(request => request.DocumentCategory).IsInEnum();
            RuleFor(request => request.DocumentType).NotEmpty();
            RuleFor(request => request.File).NotNull();
            RuleFor(request => request).Custom(ValidateRequest);
        }

        private static void ValidateRequest(UploadDocumentFileRequest request, ValidationContext<UploadDocumentFileRequest> context)
        {
            if (!request.UploadToProvider) return;
            if (!request.Provider.HasValue)
            {
                context.AddFailure(
                    nameof(request.Provider),
                    $"Provider should be specified when {nameof(request.UploadToProvider)} is enabled.");
                return;
            }

            var isSupported = request.Provider switch
            {
                ExternalFileProviderType.Onfido => IsDocumentSupportedByOnfido(request.DocumentCategory, request.DocumentType),
                _ => false
            };

            if (!isSupported)
            {
                context.AddFailure(
                    nameof(request.Provider),
                    $"Document {request.DocumentCategory} category and {request.DocumentType} can not be uploaded to {request.Provider} provider.");
            }
        }

        private static bool IsDocumentSupportedByOnfido(DocumentCategory documentCategory, string documentType) =>
            (documentCategory, documentType) switch
            {
                (DocumentCategory.ProofOfIdentity, _) => true,
                (DocumentCategory.Selfie, nameof(SelfieDocumentType.Photo)) => true,
                _ => false
            };
    }
}
