using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class Document : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DocumentCategory Category { get; set; }

        public string Type { get; set; }

        public DocumentStatus Status { get; set; }

        public HashSet<DocumentFile> Files { get; set; }
    }
}
