using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public abstract class ValidationRuleDto
    {
    }

    public class DocumentValidationRuleDto : ValidationRuleDto
    {
        public Dictionary<string, DocumentTypeValidationRuleDto> AllowedTypes { get; set; }
    }

    public class TaxResidenceValidationRuleDto : ValidationRuleDto
    {
        public string[] AllowedCountries { get; set; }
    }

    public class TinValidationRuleDto : ValidationRuleDto
    {
        public Dictionary<TinType, TinTypeValidationRuleDto> AllowedTypes { get; set; }
    }

    public class TinTypeValidationRuleDto
    {
        public string Regex { get; set; }
    }

    public class DocumentTypeValidationRuleDto
    {
        public string[] Extensions { get; set; }

        public int MaxSizeInBytes { get; set; }

        public string DescriptionCode { get; set; }

        public DocumentSide? DocumentSide { get; set; }

        public int? MaxQuantity { get; set; }

        public int? MinQuantity { get; set; }
    }
}