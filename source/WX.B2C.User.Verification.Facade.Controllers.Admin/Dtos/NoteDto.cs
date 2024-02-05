using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class NoteDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string AuthorEmail { get; set; }

        public NoteSubject Subject { get; set; }

        public Guid SubjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotRequired]
        public DateTime? UpdatedAt { get; set; }
    }
}
