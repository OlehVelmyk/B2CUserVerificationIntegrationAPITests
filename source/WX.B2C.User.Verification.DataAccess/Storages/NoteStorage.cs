using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class NoteStorage : INoteStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly INoteMapper _noteMapper;

        public NoteStorage(
            IDbContextFactory dbContextFactory,
            INoteMapper noteMapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _noteMapper = noteMapper ?? throw new ArgumentNullException(nameof(noteMapper));
        }

        public async Task<NoteDto[]> GetAsync(NoteSubject subject, Guid subjectId)
        {
            var predicate = FilterBySubjectTypeAndSubjectId(subject, subjectId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var response = await query.ToArrayAsync();

            return response?.Select(x => _noteMapper.Map(x)).ToArray();
        }

        public async Task<NoteDto> FindAsync(Guid noteId)
        {
            var predicate = FilterByNoteId(noteId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var note = await query.FirstOrDefaultAsync();

            return note != null ? _noteMapper.Map(note) : null;
        }

        private static Expression<Func<Note, bool>> FilterBySubjectTypeAndSubjectId(NoteSubject subject, Guid subjectId)
            => note => note.SubjectType == subject && note.SubjectId == subjectId;

        private static Expression<Func<Note, bool>> FilterByNoteId(Guid noteId)
            => note => note.Id == noteId;
    }
}