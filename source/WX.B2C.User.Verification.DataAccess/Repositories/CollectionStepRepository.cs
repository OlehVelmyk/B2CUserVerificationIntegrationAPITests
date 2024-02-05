using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class CollectionStepRepository : ICollectionStepRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ICollectionStepMapper _mapper;

        public CollectionStepRepository(IDbContextFactory dbContextFactory, ICollectionStepMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CollectionStep> FindNotCompletedAsync(Guid userId, string xPath)
        {
            var predicate = FilterByUser(userId)
                              .And(FilterByXPath(xPath))
                              .And(FilterNotCompleted());

            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, predicate);

            return entity == null ? null : _mapper.Map(entity);
        }

        public async Task<CollectionStep> GetAsync(Guid id)
        {
            var predicate = FilterByStep(id);

            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, predicate);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Entities.CollectionStep>(id)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(CollectionStep step)
        {
            var predicate = FilterByStep(step.Id);

            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, predicate);

            if (entity == null)
                dbContext.Add(_mapper.Map(step));
            else
            {
                _mapper.Update(step, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var predicate = FilterByStep(id);

            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, predicate);
            if (entity == null)
                throw EntityNotFoundException.ByKey<Entities.CollectionStep>(id);

            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        private static Task<Entities.CollectionStep> FindAsync(
            DbContext dbContext, Expression<Func<Entities.CollectionStep, bool>> predicate)
        {
            var query = dbContext
                        .Set<Entities.CollectionStep>()
                        .Where(predicate);

            return query.FirstOrDefaultAsync();
        }

        private static Expression<Func<Entities.CollectionStep, bool>> FilterByStep(Guid collectionStepId) =>
            step => step.Id == collectionStepId;

        private static Expression<Func<Entities.CollectionStep, bool>> FilterByUser(Guid userId) =>
            step => step.UserId == userId;

        private static Expression<Func<Entities.CollectionStep, bool>> FilterByXPath(string xPath) =>
            step => step.XPath == xPath;

        private static Expression<Func<Entities.CollectionStep, bool>> FilterNotCompleted() =>
            step => step.State != CollectionStepState.Completed 
                 && step.State != CollectionStepState.Cancelled;
    }
}