using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IValidationRuleMapper
    {
        Dictionary<ActionType, ValidationRuleDto> Map(ValidationPolicy[] validationPolicies);
    }

    internal class ValidationRuleMapper : IValidationRuleMapper
    {
        public Dictionary<ActionType, ValidationRuleDto> Map(ValidationPolicy[] validationPolicies)
        {
            if (validationPolicies == null)
                throw new ArgumentNullException(nameof(validationPolicies));

            var mappedPolicies = validationPolicies.Select(Map);
            var aggregatedPolicy = mappedPolicies.Aggregate((seed, newPolicy) =>
            {
                newPolicy.Foreach(actionRule => seed.TryAdd(actionRule.Key, actionRule.Value));
                return seed;
            });

            return aggregatedPolicy;
        }

        private Dictionary<ActionType, ValidationRuleDto> Map(ValidationPolicy policy) =>
            policy.Rules
                  .GroupBy(validation => MapToActionType(
                           validation.Rule.RuleType,
                           validation.Rule.RuleSubject),
                           validation => validation.Rule)
                  .ToDictionary(rules => rules.Key,
                                rules => MapValidationRules(rules.Key, rules));

        private static ActionType MapToActionType(string ruleType, string ruleSubject) =>
            (ruleType, ruleSubject) switch
            {
                (nameof(ActionType.Tin), _) => ActionType.Tin,
                (nameof(ActionType.TaxResidence), _) => ActionType.TaxResidence,
                (nameof(DocumentCategory.ProofOfIdentity), _) => ActionType.ProofOfIdentity,
                (nameof(DocumentCategory.ProofOfAddress), _) => ActionType.ProofOfAddress,
                (nameof(DocumentCategory.ProofOfFunds), _) => ActionType.ProofOfFunds,
                (nameof(DocumentCategory.Selfie), _) => ActionType.Selfie,
                (nameof(DocumentCategory.Taxation), nameof(TaxationDocumentType.W9Form)) => ActionType.W9Form,
                _ => throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, "Unsupported rule type.")
            };

        private ValidationRuleDto MapValidationRules(ActionType actionType, IEnumerable<ValidationRule> validationRules) =>
            actionType switch
            {
                ActionType.Tin => MapTinRules(validationRules),
                ActionType.TaxResidence => MapTaxResidenceRules(validationRules),
                ActionType.ProofOfIdentity => MapDocumentsRules(validationRules),
                ActionType.ProofOfAddress => MapDocumentsRules(validationRules),
                ActionType.ProofOfFunds => MapDocumentsRules(validationRules),
                ActionType.Selfie => MapDocumentsRules(validationRules),
                ActionType.W9Form => MapDocumentsRules(validationRules),
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, "Unsupported action type.")
            };

        private static TaxResidenceValidationRuleDto MapTaxResidenceRules(IEnumerable<ValidationRule> validationRules)
        {
            var validationRule = validationRules.SingleOrDefault();
            if (validationRule == null) return null;

            var validation = JsonConvert.DeserializeObject<TaxResidenceValidation>(validationRule.Validation);
            return new TaxResidenceValidationRuleDto { AllowedCountries = validation.AllowedCountries };
        }

        private static TinValidationRuleDto MapTinRules(IEnumerable<ValidationRule> validationRules)
        {
            var allowedTypes = validationRules.ToDictionary(
                validationRule => Enum.Parse<TinType>(validationRule.RuleSubject),
                validationRule =>
                {
                    var validation = JsonConvert.DeserializeObject<TinTypeValidation>(validationRule.Validation);
                    return new TinTypeValidationRuleDto { Regex = validation.Regex };
                });

            return new TinValidationRuleDto { AllowedTypes = allowedTypes };
        }

        private static DocumentValidationRuleDto MapDocumentsRules(IEnumerable<ValidationRule> validationRules)
        {
            var allowedTypes = validationRules.ToDictionary(
                validationRule => validationRule.RuleSubject,
                validationRule =>
                {
                    var validation = JsonConvert.DeserializeObject<DocumentTypeValidation>(validationRule.Validation);
                    return new DocumentTypeValidationRuleDto
                    {
                        DescriptionCode = validation.DescriptionCode,
                        DocumentSide = validation.DocumentSide,
                        Extensions = validation.FileFormats,
                        MaxSizeInBytes = validation.MaxFileSize,
                        MaxQuantity = validation.MaxFileQuantity,
                        MinQuantity = validation.MinFileQuantity,
                    };
                });

            return new DocumentValidationRuleDto { AllowedTypes = allowedTypes };
        }
    }
}