using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Services.Extensions
{
    public static class ValidationRuleExtensions
    {
        public static DocumentValidationRuleDto GetDocumentValidationRule(
            this Dictionary<ActionType, ValidationRuleDto> validationRules, ActionType actionType)
        {
            if (validationRules == null)
                throw new ArgumentNullException(nameof(validationRules));

            return validationRules.GetValueOrDefault(actionType) as DocumentValidationRuleDto;
        }

        public static TinValidationRuleDto GetTinValidationRule(
            this Dictionary<ActionType, ValidationRuleDto> validationRules)
        {
            if (validationRules == null)
                throw new ArgumentNullException(nameof(validationRules));

            return validationRules.GetValueOrDefault(ActionType.Tin) as TinValidationRuleDto;
        }

        public static TaxResidenceValidationRuleDto GetTaxResidenceValidationRule(
            this Dictionary<ActionType, ValidationRuleDto> validationRules)
        {
            if (validationRules == null)
                throw new ArgumentNullException(nameof(validationRules));

            return validationRules.GetValueOrDefault(ActionType.TaxResidence) as TaxResidenceValidationRuleDto;
        }
    }
}
