using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class SubmitDocumentRequest
    {
        public DocumentCategory Category { get; set; }

        public string Type { get; set; }

        public string[] Files { get; set; }

        public string Reason { get; set; }
    }
}