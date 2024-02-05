using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class AuditEntryDto
    {
        public Guid UserId { get; set; }

        public Guid EntryKey { get; set; }

        public EntryType EntryType { get; set; }

        public string EventType { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Data { get; set; }

        public InitiationDto Initiation { get; set; }
    }
}