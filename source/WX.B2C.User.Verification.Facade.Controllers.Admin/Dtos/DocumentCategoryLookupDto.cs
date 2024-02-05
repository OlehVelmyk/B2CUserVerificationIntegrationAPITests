using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class DocumentCategoryLookupDto
    {
        public DocumentCategory Name { get; set; }

        public string Description { get; set; }

        public DocumentTypeLookupDto[] Types { get; set; }
    }

    public class DocumentTypeLookupDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
