using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class CreateNoteRequest
    {
        public string Text { get; set; }

        public NoteSubject Subject { get; set; }

        public Guid SubjectId { get; set; }
    }
}
