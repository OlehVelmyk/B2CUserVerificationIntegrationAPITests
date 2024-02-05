using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class UploadDocumentFileRequestValidator : RequestAsyncValidator<UploadDocumentFileRequest>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidationRuleProvider _validationPolicyProvider;
        private readonly IActionTypeMapper _actionTypeMapper;

        public UploadDocumentFileRequestValidator(
            IHttpContextAccessor httpContextAccessor,
            IValidationRuleProvider validationPolicyProvider,
            IUserActionValidatorService userActionValidatorService,
            IActionTypeMapper actionTypeMapper)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _validationPolicyProvider = validationPolicyProvider ?? throw new ArgumentNullException(nameof(validationPolicyProvider));
            _actionTypeMapper = actionTypeMapper ?? throw new ArgumentNullException(nameof(actionTypeMapper));
            _ = userActionValidatorService ?? throw new ArgumentNullException(nameof(userActionValidatorService));

            RuleFor(request => request.DocumentCategory).IsInEnum();
            RuleFor(request => request.DocumentType).NotEmpty();
            RuleFor(request => request.File)
                .NotNull()
                .WithErrorCode(Constants.ErrorCodes.FileCanNotBeEmpty);

            RuleFor(request => request.File)
                .CustomAsync(ValidateFile)
                .When(request => request.File != null);

            RuleFor(request => request)
                .MustAsync(ValidateUserAction)
                .WithMessage(Constants.ErrorMessages.ActionIsNotAllowed)
                .WithErrorCode(Constants.ErrorCodes.ActionIsNotAllowed);

            Task ValidateFile(IFormFile file, ValidationContext<UploadDocumentFileRequest> context, CancellationToken cancellation) =>
                ValidateFileAsync(UserId, file, context);

            Task<bool> ValidateUserAction(UploadDocumentFileRequest request, CancellationToken token) =>
                userActionValidatorService.ValidateAsync(UserId, request.DocumentCategory, request.DocumentType);
        }

        private Guid UserId => _httpContextAccessor.HttpContext.User.GetUserId();

        private async Task ValidateFileAsync(
            Guid userId,
            IFormFile file,
            ValidationContext<UploadDocumentFileRequest> context)
        {
            var request = context.InstanceToValidate;
            var documentCategory = request.DocumentCategory;
            var documentType = request.DocumentType;

            var validationRule = await GetDocumentValidationRule(userId, documentCategory, documentType);
            if (validationRule == null)
            {
                context.Failure($"DocumentType '{documentType}' is not allowed for DocumentCategory '{documentCategory}'",
                                Constants.ErrorCodes.DocumentTypeIsNotAllowed,
                                new Dictionary<string, object>
                                {
                                    { nameof(documentType), documentType },
                                    { nameof(documentCategory), documentCategory },
                                });
                return;
            }

            var fileExtension = Path.GetExtension(file.FileName)?.Trim('.').ToLower();
            if (!validationRule.Extensions.Contains(fileExtension))
                context.Failure($"File with extenstion '{fileExtension}' is not allowed.", Constants.ErrorCodes.FileTypeNotSupported);

            if (file.Length > validationRule.MaxSizeInBytes)
                context.Failure("File length is higher than max allowed.", Constants.ErrorCodes.FileSizeTooLarge);
        }

        private async Task<DocumentTypeValidationRuleDto> GetDocumentValidationRule(Guid userId, DocumentCategory documentCategory, string documentType)
        {
            var actionType = _actionTypeMapper.Map(documentCategory, documentType);
            if (documentType == null || !actionType.HasValue) return null;

            var validationPolicy = await _validationPolicyProvider.FindAsync(userId, actionType.Value) as DocumentValidationRuleDto;
            return validationPolicy?.AllowedTypes.GetValueOrDefault(documentType);
        }
    }
}
