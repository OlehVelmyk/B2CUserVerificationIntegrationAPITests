using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Services.Extensions;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using CoreDtos = WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Mappers
{
    public interface IValidationRulesMapper
    {
        ValidationRulesDto Map(Dictionary<ActionType, CoreDtos.ValidationRuleDto> validationRules);

        DocumentValidationRuleDto MapDocumentValidationRule(ActionType actionType, CoreDtos.ValidationRuleDto validationRule);

        VerificationDetailsValidationRuleDto MapVerificationDetailsValidationRule(Dictionary<ActionType, CoreDtos.ValidationRuleDto> validationRules);
    }

    internal class ValidationRulesMapper : IValidationRulesMapper
    {
        private readonly IDocumentCategoryMapper _documentCategoryMapper;

        public ValidationRulesMapper(IDocumentCategoryMapper documentCategoryMapper)
        {
            _documentCategoryMapper = documentCategoryMapper ?? throw new ArgumentNullException(nameof(documentCategoryMapper));
        }
        
        public ValidationRulesDto Map(Dictionary<ActionType, CoreDtos.ValidationRuleDto> validationRules)
        {
            if (validationRules == null)
                throw new ArgumentNullException(nameof(validationRules));


            return new ValidationRulesDto
            {
                VerificationDetailsRule = MapVerificationDetailsValidationRule(validationRules),
                DocumentRules = validationRules
                                .Where(x => x.Value is CoreDtos.DocumentValidationRuleDto)
                                .Select(x => MapDocumentValidationRule(x.Key, x.Value))
                                .ToArray()
            };
        }

        public DocumentValidationRuleDto MapDocumentValidationRule(ActionType actionType, CoreDtos.ValidationRuleDto validationRule)
        {
            if (validationRule == null)
                throw new ArgumentNullException(nameof(validationRule));

            var documentValidationRule = validationRule as CoreDtos.DocumentValidationRuleDto;

            var validationRules = documentValidationRule?.AllowedTypes
                                                        .Select(x => Map(x.Key, x.Value))
                                                        .ToArray();
            
            var documentCategory = _documentCategoryMapper.Map(actionType);

            return new DocumentValidationRuleDto
            {
                DocumentCategory = documentCategory,
                ValidationRules = validationRules
            };
        }

        public VerificationDetailsValidationRuleDto MapVerificationDetailsValidationRule(Dictionary<ActionType, CoreDtos.ValidationRuleDto> validationRules)
        {
            if (validationRules == null)
                throw new ArgumentNullException(nameof(validationRules));

            var taxResidenceValidationRule = validationRules.GetTaxResidenceValidationRule();
            var tinValidationRule = validationRules.GetTinValidationRule();

            return new VerificationDetailsValidationRuleDto
            {
                TaxResidences = taxResidenceValidationRule?.AllowedCountries,
                TinValidationRules = tinValidationRule?.AllowedTypes.Select(x => Map(x.Key, x.Value)).ToArray(),
            };
        }

        private static TinValidationRuleDto Map(TinType tineType, CoreDtos.TinTypeValidationRuleDto validationRule)
        {
            if (validationRule == null)
                throw new ArgumentNullException(nameof(validationRule));

            return new TinValidationRuleDto
            {
                TinType = tineType,
                ValidationRegex = validationRule.Regex
            };
        }

        private static DocumentValidationRuleItemDto Map(string documentType, CoreDtos.DocumentTypeValidationRuleDto rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return new DocumentValidationRuleItemDto
            {
                DocumentType = documentType,
                Extensions = rule.Extensions,
                MaxQuantity = rule.MaxQuantity,
                MinQuantity = rule.MinQuantity,
                MaxSizeInBytes = rule.MaxSizeInBytes,
                DescriptionCode = rule.DescriptionCode,
                DocumentSide = rule.DocumentSide
            };
        }
    }
}