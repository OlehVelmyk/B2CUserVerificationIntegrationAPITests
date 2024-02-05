using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface INoteRepository
    {
        Task SaveAsync(NoteDto noteDto);

        Task DeleteAsync(Guid id);
    }
}
