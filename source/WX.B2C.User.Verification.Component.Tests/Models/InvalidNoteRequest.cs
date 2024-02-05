using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class InvalidNoteRequest : CreateNoteRequest
    {
        public InvalidNoteRequest(string text, NoteSubject subject, System.Guid subjectId)
            : base(text, subject, subjectId)
        {
        }
    }
}
