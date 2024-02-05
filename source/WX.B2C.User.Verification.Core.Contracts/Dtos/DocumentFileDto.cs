using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class DocumentFileDto
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }
        
        public string DocumentType { get; set; }

        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public string ExternalId { get; set; }

        public ExternalFileProviderType? Provider { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
