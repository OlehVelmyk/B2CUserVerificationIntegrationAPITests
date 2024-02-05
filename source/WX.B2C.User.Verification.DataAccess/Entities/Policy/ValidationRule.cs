using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class ValidationRule
    {
        public Guid Id { get; set; }

        public string RuleType { get; set; }

        public string RuleSubject { get; set; }

        public string Validation { get; set; }
    }


    public class DocumentTypeValidation
    {
        public string[] FileFormats { get; set; }

        public int MaxFileSize { get; set; }

        public string DescriptionCode { get; set; }

        public DocumentSide? DocumentSide { get; set; }

        public int? MinFileQuantity { get; set; }

        public int? MaxFileQuantity { get; set; }
    }

    public class TinTypeValidation
    {
        public string Regex { get; set; }
    }

    public class TaxResidenceValidation
    {
        public string[] AllowedCountries { get; set; }
    }
}