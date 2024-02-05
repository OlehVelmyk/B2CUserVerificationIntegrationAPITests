using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    class DocumentSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DocumentCategory Category { get; set; }

        public DocumentType Type { get; set; }

        public DocumentStatus Status { get; set; }

        public DocumentFileSpecimen[] Files { get; set; }
    }
}
