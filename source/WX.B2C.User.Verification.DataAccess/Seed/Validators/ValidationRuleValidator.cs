using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.Domain.Enums;
using ValidationRule = WX.B2C.User.Verification.DataAccess.Seed.Models.ValidationRule;

namespace WX.B2C.User.Verification.DataAccess.Seed.Validators
{
    internal class ValidationRuleValidator : BaseSeedValidator<ValidationRule>
    {
        private static readonly IDocumentTypeProvider DocumentTypeProvider = new HardcodedDocumentTypeProvider();
        private static IEnumerable<string> ValidRuleTypes = GetValidRuleTypes();

        public ValidationRuleValidator()
        {
            RuleFor(check => check.Id).NotEmpty();
            RuleFor(check => check.RuleSubject)
                .NotEmpty()
                .When(check => check.RuleType != nameof(ActionType.TaxResidence));
            RuleFor(check => check.RuleType);
            RuleFor(check => check.Validations)
                .NotNull()
                .Custom(ShouldHaveValidRuleSubject)
                .Custom(ShouldHaveValidValidations);
        }

        private static void ShouldHaveValidRuleSubject(object validations, ValidationContext<ValidationRule> ruleContext)
        {
            var validationRule = ruleContext.InstanceToValidate;

            var ruleType = validationRule.RuleType;
            if (!IsValidRuleType(ruleType))
                ruleContext.AddFailure($"Rule type {ruleType} is not expected.");

            var ruleSubject = validationRule.RuleSubject;
            if (!IsValidRuleSubject(ruleType, ruleSubject))
                ruleContext.AddFailure($"Rule subject {ruleSubject} is not expected.");
        }

        private static void ShouldHaveValidValidations(object validations, ValidationContext<ValidationRule> ruleContext)
        {
            var validationRule = ruleContext.InstanceToValidate;
            var ruleType = validationRule.RuleType;
            var ruleSubject = validationRule.RuleSubject;
            var serialized = JsonConvert.SerializeObject(validations);
            var expectedType = GetExpectedType();
            var deserializedObject = VerifyDeserialization(ruleContext, serialized, expectedType);

            if (deserializedObject == null)
                return;

            //TODO refactor to generic
            if (deserializedObject is TinTypeValidation[] tinRules)
            {
                foreach (var tinRule in tinRules)
                {
                    var validator = new TinValidationRuleValidator();
                    validator.ValidateAndThrow(tinRule);
                }
            }
            else if (deserializedObject is DocumentTypeValidation documentRule)
            {
                var documentRuleValidator = new DocumentRuleValidator();
                documentRuleValidator.ValidateAndThrow(documentRule);
            }

            Type GetExpectedType() =>
                ruleType switch
                {
                    nameof(DocumentCategory.ProofOfIdentity) => typeof(DocumentTypeValidation),
                    nameof(DocumentCategory.ProofOfAddress) => typeof(DocumentTypeValidation),
                    nameof(DocumentCategory.ProofOfFunds) => typeof(DocumentTypeValidation),
                    nameof(DocumentCategory.Selfie) => typeof(DocumentTypeValidation),
                    nameof(DocumentCategory.Taxation) => typeof(DocumentTypeValidation),
                    Models.RuleSubjects.TaxResidence => typeof(TaxResidenceValidation),
                    Models.RuleSubjects.Tin => typeof(TinTypeValidation),
                    _ => throw new ArgumentOutOfRangeException($"Unexpected entity: {ruleType} - {ruleSubject}")
                };
        }

        private static object VerifyDeserialization(ValidationContext<ValidationRule> ruleContext, string serializedObject, Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(serializedObject, type);
            }
            catch
            {
                ruleContext.AddFailure($"Cannot deserialize validations into {type.FullName}");
            }

            return null;
        }

        private static bool IsValidRuleType(string ruleType) => ValidRuleTypes.Contains(ruleType);

        private static bool IsValidRuleSubject(string ruleType, string ruleSubject)
        {
            return ruleType switch
            {
                nameof(Models.RuleSubjects.TaxResidence) => true,
                nameof(Models.RuleSubjects.Tin) => Enum.IsDefined(typeof(TinType), ruleSubject),
                nameof(DocumentCategory.ProofOfIdentity) => IsDocumentTypeValid(DocumentCategory.ProofOfIdentity, ruleSubject),
                nameof(DocumentCategory.ProofOfAddress) => IsDocumentTypeValid(DocumentCategory.ProofOfAddress, ruleSubject),
                nameof(DocumentCategory.ProofOfFunds) => IsDocumentTypeValid(DocumentCategory.ProofOfFunds, ruleSubject),
                nameof(DocumentCategory.Taxation) => IsDocumentTypeValid(DocumentCategory.Taxation, ruleSubject),
                nameof(DocumentCategory.Selfie) => IsDocumentTypeValid(DocumentCategory.Selfie, ruleSubject),
                _ => false
            };
        }

        private static bool IsDocumentTypeValid(DocumentCategory documentCategory, string documentType)
        {
            var documentTypes = DocumentTypeProvider.Get(documentCategory);
            return documentTypes.Any(x => x.Value.Equals(documentType));
        }

        private static IEnumerable<string> GetValidRuleTypes()
        {
            var validRuleTypes = new[] { Models.RuleSubjects.TaxResidence, Models.RuleSubjects.Tin };
            var documentCategories = Enum.GetNames(typeof(DocumentCategory));
            return validRuleTypes.Concat(documentCategories);
        }
    }

    internal class DocumentRuleValidator : BaseSeedValidator<DocumentTypeValidation>
    {
        public DocumentRuleValidator()
        {
            RuleFor(rule => rule.DescriptionCode).NotEmpty();
            RuleFor(rule => rule.MaxFileSize).NotEmpty();
            RuleFor(rule => rule.FileFormats).NotEmpty();
            RuleFor(rule => rule.DocumentSide)
                .Empty()
                .When(HasFileQuantity, ApplyConditionTo.CurrentValidator)
                .NotEmpty()
                .When(rule => !HasFileQuantity(rule), ApplyConditionTo.CurrentValidator);

            bool HasFileQuantity(DocumentTypeValidation rule) =>
                rule.MaxFileQuantity.HasValue && rule.MinFileQuantity.HasValue;
        }
    }

    internal class TinValidationRuleValidator : BaseSeedValidator<TinTypeValidation>
    {
        public TinValidationRuleValidator()
        {
            RuleFor(rule => rule.Regex).NotEmpty();
            RuleFor(rule => rule.Regex)
                .Custom(IsValidRegexPattern)
                .When(dto => !string.IsNullOrEmpty(dto.Regex));
        }

        private static void IsValidRegexPattern(object regex, ValidationContext<TinTypeValidation> ruleContext)
        {
            if (!IsValidRegexPattern(regex?.ToString()))
                ruleContext.AddFailure($"Regex is not valid {regex}");
        }

        private static bool IsValidRegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            var re = new Regex(pattern, RegexOptions.None);
            try
            {
                _ = re.IsMatch("");
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}