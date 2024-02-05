using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UploadDocumentFileRequest
    {
        public DocumentCategory DocumentCategory { get; set; }

        public string DocumentType { get; set; }

        public bool UploadToProvider { get; set; } = false;

        [NotRequired]
        public ExternalFileProviderType? Provider { get; set; } = ExternalFileProviderType.Onfido;

        public IFormFile File { get; set; }
    }
}
