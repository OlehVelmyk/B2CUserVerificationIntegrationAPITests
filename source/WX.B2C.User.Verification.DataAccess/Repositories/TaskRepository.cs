using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class TaskRepository : ITaskRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ITaskMapper _mapper;

        public TaskRepository(IDbContextFactory dbContextFactory, ITaskMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<VerificationTask> GetAsync(Guid id)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, id);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Entities.VerificationTask>(id)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(VerificationTask task)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, task.Id);

            if (entity == null)
                dbContext.Add(_mapper.Map(task));
            else
            {
                _mapper.Update(task, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<Entities.VerificationTask> FindAsync(DbContext dbContext, Guid id)
        {
            var query = dbContext
                        .Set<Entities.VerificationTask>()
                        .Include(x => x.PerformedChecks).ThenInclude(x => x.Check)
                        .Include(x => x.CollectionSteps).ThenInclude(x => x.Step)
                        .Where(task => task.Id == id);

            return query.SingleOrDefaultAsync();
        }
    }
}
