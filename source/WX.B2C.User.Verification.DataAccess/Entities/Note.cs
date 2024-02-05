using System;
using System.ComponentModel.DataAnnotations;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class Note : AuditableEntity
    {
        public Guid Id { get; set; }

        public NoteSubject SubjectType { get; set; }

        public Guid SubjectId { get; set; }

        public string AuthorEmail { get; set; }

        public string Text { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}