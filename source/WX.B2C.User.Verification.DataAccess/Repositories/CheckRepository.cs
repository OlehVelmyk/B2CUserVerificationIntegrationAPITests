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
    internal class CheckRepository : ICheckRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ICheckMapper _mapper;

        public CheckRepository(IDbContextFactory dbContextFactory, ICheckMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Check> GetAsync(Guid checkId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, checkId);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Check>(checkId)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(Check check)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, check.Id);

            if (entity == null)
            {
                dbContext.Add(_mapper.Map(check));
                foreach (var taskCheck in _mapper.MapToTasksChecks(check))
                    dbContext.Add(taskCheck);
            }
            else
            {
                _mapper.Update(check, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<Entities.Check> FindAsync(DbContext dbContext, Guid id)
        {
            var query = dbContext
                        .Set<Entities.Check>()
                        .Where(check => check.Id == id);

            return query.SingleOrDefaultAsync();
        }
    }
}

