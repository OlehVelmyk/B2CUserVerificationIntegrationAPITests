using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class DocumentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DocumentCategory Category { get; set; }

        public string Type { get; set; }

        public DocumentStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public DocumentFileDto[] Files { get; set; }
    }
}
