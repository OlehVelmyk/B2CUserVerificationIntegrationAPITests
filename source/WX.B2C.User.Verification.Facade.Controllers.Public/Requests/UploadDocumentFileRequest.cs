using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Requests
{
    public class UploadDocumentFileRequest
    {
        [ModelBinder(Name = "document_category")]
        public DocumentCategory DocumentCategory { get; set; }

        [ModelBinder(Name = "document_type")]
        public string DocumentType { get; set; }

        [ModelBinder(Name = "file")]
        public IFormFile File { get; set; }
    }
}