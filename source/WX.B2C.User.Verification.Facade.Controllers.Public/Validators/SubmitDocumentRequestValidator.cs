using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class SubmitDocumentRequestValidator : RequestAsyncValidator<SubmitDocumentRequest>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidationRuleProvider _validationPolicyProvider;
        private readonly IActionTypeMapper _actionTypeMapper;

        public SubmitDocumentRequestValidator(
            IHttpContextAccessor httpContextAccessor,
            IValidationRuleProvider validationPolicyProvider,
            IUserActionValidatorService userActionValidatorService,
            IActionTypeMapper actionTypeMapper)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _validationPolicyProvider = validationPolicyProvider ?? throw new ArgumentNullException(nameof(validationPolicyProvider));
            _actionTypeMapper = actionTypeMapper ?? throw new ArgumentNullException(nameof(actionTypeMapper));
            _ = userActionValidatorService ?? throw new ArgumentNullException(nameof(userActionValidatorService));

            RuleFor(request => request.Category).IsInEnum();
            RuleFor(request => request.Type).NotEmpty();
            RuleFor(request => request.Provider)
                .IsInEnum()
                .When(request => request.Provider != null)
                .WithMessage("Provider should be specified.");

            RuleFor(request => request.Files).NotEmpty();
            RuleForEach(request => request.Files)
                .NotEmpty()
                .WithMessage("FileIds should be specified.");

            RuleFor(request => request.Files)
                .CustomAsync(ValidateFiles)
                .When(request => !request.Type.IsNullOrEmpty() && request.Files != null);

            RuleFor(request => request)
                .MustAsync(ValidateUserAction)
                .WithMessage(Constants.ErrorMessages.ActionIsNotAllowed)
                .WithErrorCode(Constants.ErrorCodes.ActionIsNotAllowed);

            Task ValidateFiles(string[] files, ValidationContext<SubmitDocumentRequest> context, CancellationToken cancellation) =>
                ValidateFilesAsync(UserId, files, context);

            Task<bool> ValidateUserAction(SubmitDocumentRequest request, CancellationToken token) =>
                userActionValidatorService.ValidateAsync(UserId, request.Category, request.Type);
        }

        private Guid UserId => _httpContextAccessor.HttpContext.User.GetUserId();

        private async Task ValidateFilesAsync(
            Guid userId,
            string[] files,
            ValidationContext<SubmitDocumentRequest> context)
        {
            var request = context.InstanceToValidate;
            var documentCategory = request.Category;
            var documentType = request.Type;

            var validationRule = await GetDocumentValidationRule(userId, documentCategory, documentType);
            if (validationRule == null)
            {
                context.Failure($"DocumentType '{documentType}' is not allowed for DocumentCategory '{documentCategory}'.", Constants.ErrorCodes.DocumentTypeIsNotAllowed, new Dictionary<string, object>
                {
                    { nameof(documentType), documentType },
                    { nameof(documentCategory), documentCategory },
                });
                return;
            }

            if (files.Length < validationRule.MinQuantity)
                context.Failure($"Document should have at least {validationRule.MinQuantity ?? 1} files.", Constants.ErrorCodes.IncorrectFilesQuantity);
            if (files.Length > validationRule.MaxQuantity)
                context.Failure($"Amount of files is bigger than max allowed value '{validationRule.MaxQuantity}'.", Constants.ErrorCodes.IncorrectFilesQuantity);

            var specificQuantity = GetSpecificQuantity(validationRule);
            if (specificQuantity.HasValue && files.Length != specificQuantity)
                context.Failure($"Amount of files should be {specificQuantity}.", Constants.ErrorCodes.IncorrectFilesQuantity);
        }

        private static int? GetSpecificQuantity(DocumentTypeValidationRuleDto validationRule)
        {
            if (validationRule.DocumentSide is DocumentSide.Front or DocumentSide.Back) return 1;
            if (validationRule.DocumentSide is DocumentSide.Both) return 2;

            return null;
        }

        private async Task<DocumentTypeValidationRuleDto> GetDocumentValidationRule(Guid userId, DocumentCategory documentCategory, string documentType)
        {
            var actionType = _actionTypeMapper.Map(documentCategory, documentType);
            if (!actionType.HasValue) return null;

            var documentValidationRule = await _validationPolicyProvider.FindAsync(userId, actionType.Value) as DocumentValidationRuleDto;
            return documentValidationRule?.AllowedTypes.GetValueOrDefault(documentType);
        }
    }
}
