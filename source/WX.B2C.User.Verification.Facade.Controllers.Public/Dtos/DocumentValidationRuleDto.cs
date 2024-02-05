using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    public class DocumentValidationRuleDto
    {
        public DocumentCategory DocumentCategory { get; set; }

        public DocumentValidationRuleItemDto[] ValidationRules { get; set; }
    }

    public class DocumentValidationRuleItemDto
    {
        public string DocumentType { get; set; }

        public string[] Extensions { get; set; }

        public string DescriptionCode { get; set; }

        public int MaxSizeInBytes { get; set; }

        [NotRequired]
        public DocumentSide? DocumentSide { get; set; }

        [NotRequired]
        public int? MaxQuantity { get; set; }

        [NotRequired]
        public int? MinQuantity { get; set; }
    }
}
