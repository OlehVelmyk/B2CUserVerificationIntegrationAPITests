using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface INoteMapper
    {
        Note Map(NoteDto note);

        NoteDto Map(Note note);
    }

    internal class NoteMapper : INoteMapper
    {
        public Note Map(NoteDto noteDto)
        {
            if (noteDto == null)
                throw new ArgumentNullException(nameof(noteDto));

            return new Note
            {
                Id = noteDto.Id,
                AuthorEmail = noteDto.AuthorEmail,
                SubjectType = noteDto.Subject,
                SubjectId = noteDto.SubjectId,
                Text = noteDto.Text,
                CreatedAt = noteDto.CreatedAt
            };
        }

        public NoteDto Map(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            return new NoteDto
            {
                Id = note.Id,
                Subject = note.SubjectType,
                SubjectId = note.SubjectId,
                AuthorEmail = note.AuthorEmail,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
        }
    }
}