using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class NoteDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string AuthorEmail { get; set; }

        public NoteSubject Subject { get; set; }

        public Guid SubjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}