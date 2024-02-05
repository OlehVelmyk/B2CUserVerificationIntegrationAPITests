using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Extensions;
using CheckState = WX.B2C.User.Verification.Domain.Models.CheckState;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class CheckStorage : ICheckStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ICheckMapper _mapper;

        public CheckStorage(IDbContextFactory dbContextFactory, ICheckMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CheckDto[]> FindByExternalIdAsync(string externalId, Domain.Models.CheckProviderType provider)
        {
            var predicate = FilterByExternalId(externalId).And(FilterByProvider(provider));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var checks = await query.ToArrayAsync();

            return checks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<CheckDto[]> GetAllAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var checks = await query.ToArrayAsync();

            return checks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<CheckDto[]> GetPendingAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId).And(FilterByState(CheckState.Pending));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var checks = await query.ToArrayAsync();

            return checks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<CheckDto[]> GetAsync(Guid userId , Guid[] variantIds)
        {
            var predicate = FilterByVariantIds(variantIds).And(FilterByUserId(userId));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var checks = await query.ToArrayAsync();

            return checks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<CheckDto> GetAsync(Guid checkId)
        {
            var predicate = FilterById(checkId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var check = await query.FirstOrDefaultAsync();

            return check == null 
                ? throw EntityNotFoundException.ByKey<Check>(checkId) 
                : _mapper.MapToDto(check);
        }

        public async Task<CheckDto> FindAsync(Guid checkId, Guid userId)
        {
            var predicate = FilterById(checkId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var check = await query.FirstOrDefaultAsync();

            return check?.UserId != userId ? null : _mapper.MapToDto(check);
        }

        public async Task<Guid[]> GetRelatedTasksAsync(Guid checkId)
        {
            Expression<Func<TaskCheck, bool>> predicate = taskCheck => taskCheck.CheckId == checkId;

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var taskChecks = await query.ToArrayAsync();

            return taskChecks.Select(taskCheck => taskCheck.TaskId).ToArray();
        }

        private static Expression<Func<Check, bool>> FilterById(Guid checkId) =>
            check => check.Id == checkId;

        private static Expression<Func<Check, bool>> FilterByVariantIds(IEnumerable<Guid> variantIds) =>
            check => variantIds.Contains(check.VariantId);

        private static Expression<Func<Check, bool>> FilterByUserId(Guid userId) =>
            check => check.UserId == userId;

        private static Expression<Func<Check, bool>> FilterByExternalId(string externalId) =>
            check => check.ExternalId == externalId;

        private static Expression<Func<Check, bool>> FilterByProvider(Domain.Models.CheckProviderType provider) =>
            check => check.Provider == provider;

        private static Expression<Func<Check, bool>> FilterByState(CheckState state) =>
            check => check.State == state;
    }
}