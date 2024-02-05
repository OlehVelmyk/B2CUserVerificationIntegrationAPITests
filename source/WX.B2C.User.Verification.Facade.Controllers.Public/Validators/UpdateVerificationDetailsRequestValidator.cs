using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Core.Services.Extensions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class UpdateVerificationDetailsRequestValidator : RequestAsyncValidator<UpdateVerificationDetailsRequest>
    {
        private delegate IEnumerable<ValidationFailure> PropertyValidator(UpdateVerificationDetailsRequest request, Dictionary<ActionType, ValidationRuleDto> validationRules);
        private readonly IDictionary<string, PropertyValidator> _propertyValidators = new Dictionary<string, PropertyValidator>
        {
            [nameof(UpdateVerificationDetailsRequest.Tin)] = (request, validationRules) =>
                ValidateTin(request.Tin, validationRules.GetTinValidationRule()),
            [nameof(UpdateVerificationDetailsRequest.TaxResidence)] = (request, validationRules) =>
                ValidateTaxResidences(request.TaxResidence, validationRules.GetTaxResidenceValidationRule())
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidationRuleProvider _validationPolicyProvider;
        private readonly IUserActionValidatorService _userActionValidatorService;

        public UpdateVerificationDetailsRequestValidator(
            IHttpContextAccessor httpContextAccessor,
            IValidationRuleProvider validationPolicyProvider,
            IUserActionValidatorService userActionValidatorService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _validationPolicyProvider = validationPolicyProvider ?? throw new ArgumentNullException(nameof(validationPolicyProvider));
            _userActionValidatorService = userActionValidatorService ?? throw new ArgumentNullException(nameof(userActionValidatorService));

            RuleFor(request => request.Tin.Type)
                .IsInEnum()
                .When(request => request.Tin != null);

            RuleFor(request => request.TaxResidence)
                .OnlyUniqueValues()
                .When(request => request.TaxResidence != null);

            RuleFor(request => request)
                .CustomAsync(ValidateRequestAsync);
        }

        private async Task ValidateRequestAsync(
            UpdateVerificationDetailsRequest request,
            ValidationContext<UpdateVerificationDetailsRequest> context,
            CancellationToken cancellation = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext.User.GetUserId();

            var propertiesToValidate = new List<string>();
            if (request.Tin != null)
                propertiesToValidate.Add(nameof(request.Tin));
            if (!request.TaxResidence.IsNullOrEmpty())
                propertiesToValidate.Add(nameof(request.TaxResidence));

            if (!propertiesToValidate.Any())
                context.AddFailure("At least one field should be defined.");

            var actionTypes = propertiesToValidate.ToDictionary(prop => prop, MapToAction);
            var validationRules = await _validationPolicyProvider.GetAsync(userId, actionTypes.Values.ToArray());

            foreach (var (propertyName, actionType) in actionTypes)
            {
                var isActionAllowed = await ValidateAction(actionType);
                if (!isActionAllowed)
                    context.Failure(actionType.ToString(), Constants.ErrorMessages.ActionIsNotAllowed, Constants.ErrorCodes.ActionIsNotAllowed);

                var propertyValidator = _propertyValidators[propertyName];
                var validationFailures = propertyValidator(request, validationRules);
                foreach (var validationFailure in validationFailures)
                    context.AddFailure(validationFailure);
            }

            Task<bool> ValidateAction(ActionType actionType) => _userActionValidatorService.ValidateAsync(userId, actionType);
        }

        private static IEnumerable<ValidationFailure> ValidateTin(Dtos.TinDto tin, TinValidationRuleDto validationRule)
        {
            var tinValidation = validationRule?.AllowedTypes.GetValueOrDefault(tin.Type);
            if (tinValidation == null)
                yield return new ValidationFailure(nameof(tin), $"TinType: {tin.Type} is not allowed.");
            else if (!Regex.IsMatch(tin.Number, tinValidation.Regex))
                yield return new ValidationFailure(nameof(tin), $"Tin number {tin.Number} is not valid, regex: {tinValidation.Regex}.");
        }

        private static IEnumerable<ValidationFailure> ValidateTaxResidences(IEnumerable<string> taxResidences, TaxResidenceValidationRuleDto validationRule)
        {
            var allowedCountries = validationRule?.AllowedCountries ?? Array.Empty<string>();
            var notAllowed = taxResidences.Where(taxResidence => allowedCountries.All(countryCode => countryCode != taxResidence));
            return notAllowed.Select(x => new ValidationFailure(nameof(taxResidences), $"Residence country '{x}' is not allowed")).ToArray();
        }

        private static ActionType MapToAction(string propertyName)
        {
            return propertyName switch
            {
                nameof(UpdateVerificationDetailsRequest.Tin) => ActionType.Tin,
                nameof(UpdateVerificationDetailsRequest.TaxResidence) => ActionType.TaxResidence,
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported request property.")
            };
        }
    }
}