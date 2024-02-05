using System;
using System.Collections.Generic;
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
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;
using Trigger = WX.B2C.User.Verification.DataAccess.Entities.Trigger;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class TriggerStorage : ITriggerStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ITriggerMapper _mapper;

        public TriggerStorage(IDbContextFactory dbContextFactory, ITriggerMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TriggerDto> GetAsync(Guid triggerId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var predicate = ByTriggerId(triggerId);
            var trigger = await dbContext.QueryAsNoTracking(predicate)
                                         .FirstOrDefaultAsync();

            return trigger == null
                ? throw EntityNotFoundException.ByKey<TriggerVariant>(triggerId)
                : _mapper.MapToDto(trigger);
        }

        public async Task<TriggerDto[]> GetAllAsync(Guid applicationId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var predicate = ByApplicationId(applicationId);
            var triggers = await dbContext.QueryAsNoTracking(predicate)
                                         .ToArrayAsync();

            return triggers.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<Dictionary<string, object>> FindLastContextAsync(Guid variantId, Guid applicationId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var predicate = ByVariantId(variantId).And(ByApplicationId(applicationId)).And(ByState(TriggerState.Completed));
            var trigger = await dbContext.QueryAsNoTracking(predicate)
                                         .OrderByDescending(t => t.FiringDate)
                                         .FirstOrDefaultAsync();

            return trigger?.Context;
        }

        private static Expression<Func<Trigger, bool>> ByApplicationId(Guid applicationId) =>
            trigger => trigger.ApplicationId == applicationId;

        private static Expression<Func<Trigger, bool>> ByTriggerId(Guid triggerId) =>
            trigger => trigger.Id == triggerId;

        private static Expression<Func<Trigger, bool>> ByVariantId(Guid variantId) =>
            trigger => trigger.VariantId == variantId;

        private static Expression<Func<Trigger, bool>> ByState(TriggerState triggerState) =>
            trigger => trigger.State == triggerState;
    }
}