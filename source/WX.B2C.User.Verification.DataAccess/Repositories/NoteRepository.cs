using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class NoteRepository : INoteRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly INoteMapper _noteMapper;

        public NoteRepository(
            IDbContextFactory dbContextFactory,
            INoteMapper noteMapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _noteMapper = noteMapper ?? throw new ArgumentNullException(nameof(noteMapper));
        }

        public async Task SaveAsync(NoteDto noteDto)
        {
            var entity = _noteMapper.Map(noteDto);

            await using var dbContext = _dbContextFactory.Create();
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var dbContext = _dbContextFactory.Create();

            var note = await FindAsync(dbContext, id);
            if (note == null)
                throw EntityNotFoundException.ByKey<Note>(id);

            dbContext.Remove(note);
            await dbContext.SaveChangesAsync();
        }

        private static Task<Note> FindAsync(DbContext dbContext, Guid id)
        {
            var query = dbContext
                        .Set<Note>()
                        .Where(x => x.Id == id);

            return query.SingleOrDefaultAsync();
        }
    }
}