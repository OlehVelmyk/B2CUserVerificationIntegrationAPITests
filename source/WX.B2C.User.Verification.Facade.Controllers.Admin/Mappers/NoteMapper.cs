using System;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using CoreNoteDto = WX.B2C.User.Verification.Core.Contracts.Dtos.NoteDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface INoteMapper
    {
        CoreNoteDto Map(CreateNoteRequest request, string authorEmail);

        NoteDto Map(CoreNoteDto note);
    }

    internal class NoteMapper : INoteMapper
    {
        public CoreNoteDto Map(CreateNoteRequest request, string authorEmail)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (authorEmail == null)
                throw new ArgumentNullException(nameof(authorEmail));

            return new CoreNoteDto
            {
                SubjectId = request.SubjectId,
                Subject = request.Subject,
                Text = request.Text,
                AuthorEmail = authorEmail
            };
        }

        public NoteDto Map(CoreNoteDto note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            return new NoteDto
            {
                Id = note.Id,
                AuthorEmail = note.AuthorEmail,
                SubjectId = note.SubjectId,
                Subject = note.Subject,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
        }
    }
}
