using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class SubmitDocumentDto
    {
        public DocumentCategory Category { get; set; }

        public string Type { get; set; }

        public Guid[] FileIds { get; set; }
    }
}