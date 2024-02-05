using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface INoteService
    {
        Task<Guid> CreateAsync(NoteDto noteDto);

        Task DeleteAsync(Guid noteId);
    }
}