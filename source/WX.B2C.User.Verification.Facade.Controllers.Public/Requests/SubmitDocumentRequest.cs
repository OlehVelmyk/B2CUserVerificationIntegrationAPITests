using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Requests
{
    public class SubmitDocumentRequest
    {
        public DocumentCategory Category { get; set; }

        public string Type { get; set; }

        [NotRequired]
        public ExternalFileProviderType? Provider { get; set; }

        public string[] Files { get; set; }
    }
}