using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class TriggerRepository : ITriggerRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ITriggerMapper _mapper;

        public TriggerRepository(IDbContextFactory dbContextFactory, ITriggerMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Trigger> FindNotFiredAsync(Guid triggerVariantId, Guid applicationId)
        {
            var predicate = WithApplicationId(applicationId).
                            And(WithPolicyId(triggerVariantId)).
                            And(WithNotState(TriggerState.Fired)).
                            And(WithNotState(TriggerState.Completed));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryLatest(dbContext, predicate);

            var entity = await query.FirstOrDefaultAsync();

            return entity == null ? null : _mapper.Map(entity);
        }

        public async Task<Trigger> GetAsync(Guid triggerId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, triggerId);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Trigger>(triggerId)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(Trigger trigger)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, trigger.Id);

            if (entity == null)
                dbContext.Add(_mapper.Map(trigger));
            else
            {
                _mapper.Update(trigger, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task<Entities.Trigger> FindAsync(DbContext dbContext, Guid triggerId)
        {
            var predicate = WithTriggerId(triggerId);
            var query = dbContext.Set<Entities.Trigger>();
            return await query.FirstOrDefaultAsync(predicate);
        }

        private static IOrderedQueryable<Entities.Trigger> QueryLatest(DbContext dbContext, 
                                                                       Expression<Func<Entities.Trigger, bool>> predicate) =>
            dbContext.QueryAsNoTracking(predicate)
                     .OrderByDescending(trigger => trigger.FiringDate);

        private static Expression<Func<Entities.Trigger, bool>> WithApplicationId(Guid applicationId) =>
            task => task.ApplicationId == applicationId;

        private static Expression<Func<Entities.Trigger, bool>> WithPolicyId(Guid variantId) =>
            task => task.VariantId == variantId;

        private static Expression<Func<Entities.Trigger, bool>> WithTriggerId(Guid id) =>
            task => task.Id == id;

        private static Expression<Func<Entities.Trigger, bool>> WithNotState(TriggerState state) =>
            task => task.State != state;
    }
}
