using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class TriggerVariantStorage : ITriggerVariantStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ITriggerVariantMapper _mapper;

        public TriggerVariantStorage(IDbContextFactory dbContextFactory, ITriggerVariantMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TriggerVariantDto[]> FindAsync(Guid policyId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var predicate = ByPolicyId(policyId);
            var triggers = await dbContext.QueryAsNoTracking(predicate)
                                          .ToArrayAsync();

            return triggers.Select(_mapper.Map).ToArray();
        }

        public async Task<TriggerVariantDto> GetAsync(Guid triggerVariantId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var predicate = ByTriggerId(triggerVariantId);
            var trigger = await dbContext.QueryAsNoTracking(predicate)
                                         .FirstOrDefaultAsync();

            return trigger == null
                ? throw EntityNotFoundException.ByKey<TriggerVariant>(triggerVariantId)
                : _mapper.Map(trigger);
        }

        private Expression<Func<TriggerVariant, bool>> ByPolicyId(Guid policyId)
        {
            return trigger => trigger.PolicyId == policyId;
        }

        private Expression<Func<TriggerVariant, bool>> ByTriggerId(Guid triggerId)
        {
            return trigger => trigger.Id == triggerId;
        }
    }
}