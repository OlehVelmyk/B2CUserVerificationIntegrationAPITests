using System;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos
{
    public class DocumentDto
    {
        public Guid Id { get; set; }

        [StringEnum(typeof(DocumentCategory))]
        public string Category { get; set; }

        public string Type { get; set; }

        public DateTime SubmittedAt { get; set; }

        public DocumentFileDto[] Files { get; set; }
    }

    public class DocumentFileDto
    {
        public string FileName { get; set; }

        public string DownloadHref { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}