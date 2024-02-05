using FluentValidation;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class CollectionStepRequestValidator : BaseRequestValidator<CollectionStepRequest>
    {
        public CollectionStepRequestValidator()
        {
            RuleFor(request => request.Type).IsInEnum();
            RuleFor(request => request.TargetTasks).NotEmpty();
            RuleFor(request => request.Reason).NotEmpty();
            RuleFor(request => request).Must(BeValidRequestType);
        }

        private static bool BeValidRequestType(CollectionStepRequest request)
        {
            var expectedType = request.Type switch
            {
                CollectionStepType.PersonalDetails => typeof(PersonalDetailsCollectionStepRequest),
                CollectionStepType.VerificationDetails => typeof(VerificationDetailsCollectionStepRequest),
                CollectionStepType.Document => typeof(DocumentCollectionStepRequest),
                CollectionStepType.Survey => typeof(SurveyCollectionStepRequest),
                _ => null
            };

            return expectedType != null && request.GetType() == expectedType;
        }
    }

    public class DocumentStepRequestValidator : BaseRequestValidator<DocumentCollectionStepRequest>
    {
        private static readonly string WrongSelfieType =
            $"{nameof(DocumentCollectionStepRequest.DocumentType)} must be "
          + $"{SelfieDocumentType.Photo.Value} or {SelfieDocumentType.Video.Value} "
          + $"when {nameof(DocumentCollectionStepRequest.DocumentCategory)} is {DocumentCategory.Selfie}";

        private static readonly string WrongTaxationType =
            $"{nameof(DocumentCollectionStepRequest.DocumentType)} must be "
          + $"{TaxationDocumentType.W9Form.Value}"
          + $"when {nameof(DocumentCollectionStepRequest.DocumentCategory)} is {DocumentCategory.Taxation}";

        public DocumentStepRequestValidator()
        {
            Include(new CollectionStepRequestValidator());
            RuleFor(request => request.DocumentCategory).IsInEnum();

            RuleFor(request => request.DocumentType)
                .Must(BeValidSelfieType)
                .When(request => request.DocumentCategory == DocumentCategory.Selfie)
                .WithMessage(WrongSelfieType);

            RuleFor(request => request.DocumentType)
                .Must(BeValidTaxationType)
                .When(request => request.DocumentCategory == DocumentCategory.Taxation)
                .WithMessage(WrongTaxationType);

            bool BeValidSelfieType(string documentType) =>
                string.Equals(documentType, SelfieDocumentType.Photo.Value) ||
                string.Equals(documentType, SelfieDocumentType.Video.Value);

            bool BeValidTaxationType(string documentType) => string.Equals(documentType, TaxationDocumentType.W9Form);
        }
    }

    public class SurveyCollectionStepRequestValidator : BaseRequestValidator<SurveyCollectionStepRequest>
    {
        public SurveyCollectionStepRequestValidator()
        {
            Include(new CollectionStepRequestValidator());
            RuleFor(request => request.TemplateId).NotEmpty();
        }
    }

    public class PersonalDetailsCollectionStepRequestValidator : BaseRequestValidator<PersonalDetailsCollectionStepRequest>
    {
        public PersonalDetailsCollectionStepRequestValidator()
        {
            Include(new CollectionStepRequestValidator());
            RuleFor(request => request.PersonalProperty).IsInEnum();
        }
    }

    public class VerificationDetailsCollectionStepRequestValidator : BaseRequestValidator<VerificationDetailsCollectionStepRequest>
    {
        public VerificationDetailsCollectionStepRequestValidator()
        {
            Include(new CollectionStepRequestValidator());
            RuleFor(request => request.VerificationProperty).IsInEnum();
        }
    }
}
