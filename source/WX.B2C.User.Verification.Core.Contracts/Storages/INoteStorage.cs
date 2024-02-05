using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface INoteStorage
    {
        Task<NoteDto[]> GetAsync(NoteSubject subject, Guid subjectId);

        Task<NoteDto> FindAsync(Guid noteId);
    }
}
