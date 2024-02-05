using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;

namespace WX.B2C.User.Verification.Core.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        }

        public async Task<Guid> CreateAsync(NoteDto noteDto)
        {
            noteDto.Id = Guid.NewGuid();
            await _noteRepository.SaveAsync(noteDto);
            return noteDto.Id;
        }

        public Task DeleteAsync(Guid noteId) => _noteRepository.DeleteAsync(noteId);
    }
}