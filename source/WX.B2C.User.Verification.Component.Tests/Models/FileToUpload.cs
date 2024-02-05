using System;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class FileToUpload
    {
        public FileToUpload(DocumentCategory documentCategory, string documentType, FileData data)
        {
            DocumentCategory = documentCategory;
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            UploadToOnfido = documentCategory == DocumentCategory.ProofOfIdentity || documentType == DocumentTypes.Photo;
        }

        public DocumentCategory DocumentCategory { get; }

        public string DocumentType { get; }

        public bool UploadToOnfido { get; }

        public FileData Data { get; }
    }
}