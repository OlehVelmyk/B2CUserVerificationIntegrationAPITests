using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class VerificationPolicyStorage : IVerificationPolicyStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IPolicyMapper _policyMapper;

        public VerificationPolicyStorage(IDbContextFactory dbContextFactory, IPolicyMapper policyMapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _policyMapper = policyMapper ?? throw new ArgumentNullException(nameof(policyMapper));
        }
        public async Task<VerificationPolicyDto[]> GetAsync(IEnumerable<Guid> policyIds)
        {
            var predicate = FilterByPolicyIds(policyIds);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryPolicy(dbContext, predicate);
            var policies = await query.ToArrayAsync();

            return policies.Select(_policyMapper.Map).ToArray();
        }

        public async Task<VerificationPolicyDto> GetAsync(Guid policyId)
        {
            var predicate = FilterByPolicyId(policyId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryPolicy(dbContext, predicate);
            var policy = await query.FirstOrDefaultAsync();

            return policy == null
                ? throw EntityNotFoundException.ByKey<VerificationPolicy>(policyId)
                : _policyMapper.Map(policy);
        }

        public async Task<VerificationPolicyDto> GetAsync(VerificationPolicySelectionContext selectionContext)
        {
            var predicate = FilterBySelectionContext(selectionContext);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryPolicy(dbContext, predicate);
            var policies = await query.ToArrayAsync();

            var verificationPolicy = policies
                                     .OrderByDescending(policy => policy.RegionType)
                                     .FirstOrDefault();

            return verificationPolicy == null
                ? throw EntityNotFoundException.ByQuery<VerificationPolicy>(selectionContext)
                : _policyMapper.Map(verificationPolicy);
        }

        public async Task<Guid> GetIdAsync(VerificationPolicySelectionContext selectionContext)
        {
            var policyId = await FindIdAsync(selectionContext);
            return policyId ?? throw EntityNotFoundException.ByQuery<VerificationPolicy>(selectionContext);
        }

        public async Task<Guid?> FindIdAsync(VerificationPolicySelectionContext selectionContext)
        {
            var predicate = FilterBySelectionContext(selectionContext);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(policy => policy.RegionType)
                        .Select(policy => (Guid?)policy.Id);
            var policyId = await query.FirstOrDefaultAsync();

            return policyId;
        }

        public async Task<CheckVariantInfo[]> GetChecksInfoAsync(Guid[] variantIds = null)
        {
            Expression<Func<PolicyCheckVariant, bool>> predicate =
                variantIds == null ? null : check => variantIds.Contains(check.Id);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var checks = await query.ToArrayAsync();

            var missingChecks = variantIds?.Except(checks.Select(step => step.Id)).ToArray();
            return missingChecks?.Length > 0
                ? throw EntityNotFoundException.ByKeys<CheckVariantInfo>(missingChecks)
                : checks.Select(_policyMapper.Map).ToArray();
        }

        public async Task<CheckVariantInfo> FindCheckInfoAsync(Guid variantId)
        {
            var predicate = FilterByVariantId();

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var check = await query.FirstOrDefaultAsync();

            return check == null ? null : _policyMapper.Map(check);

            Expression<Func<PolicyCheckVariant, bool>> FilterByVariantId() =>
                checkVariant => checkVariant.Id == variantId;
        }

        public async Task<VariantNameDto[]> GetCheckVariantNamesAsync(Guid[] variantIds)
        {
            if (!variantIds.Any())
                return Array.Empty<VariantNameDto>();

            variantIds = variantIds.Distinct().ToArray();
            var predicate = FilterByCheckVariantIds(variantIds);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .Select(x => new { x.Id, x.Name });
            var checkVariants = await query.ToArrayAsync();

            var missingVariants = variantIds.Except(checkVariants.Select(step => step.Id)).ToArray();
            return missingVariants.Length > 0
                ? throw EntityNotFoundException.ByKeys<TaskVariant>(missingVariants)
                : checkVariants.Select(a => _policyMapper.Map(a.Id, a.Name)).ToArray();
        }

        public async Task<AutoCompletePolicy> GetTaskAutoCompletePolicyAsync(Guid taskVariantId)
        {
            var predicate = BuildPredicate();

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .Select(t => new { t.Id, t.AutoCompletePolicy });
            var task = await query.FirstOrDefaultAsync();

            return task?.AutoCompletePolicy ?? throw EntityNotFoundException.ByKey<TaskVariant>(taskVariantId);

            Expression<Func<TaskVariant, bool>> BuildPredicate() => policyTask => policyTask.Id == taskVariantId;
        }

        public async Task<TaskVariantDto> GetTaskVariantAsync(Guid taskVariantId)
        {
            var predicate = FilterByTaskVariantId(taskVariantId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryTaskVariant(dbContext, predicate);
            var task = await query.FirstOrDefaultAsync();

            return task == null
                ? throw EntityNotFoundException.ByKey<TaskVariant>(taskVariantId)
                : _policyMapper.Map(task);
        }

        public async Task<TaskVariantDto[]> GetTaskVariantsAsync(Guid[] taskVariantIds)
        {
            var predicate = FilterByTaskVariantIds(taskVariantIds);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryTaskVariant(dbContext, predicate);
            var taskVariants = await query.ToArrayAsync();

            var missingVariants = taskVariantIds.Except(taskVariants.Select(step => step.Id)).ToArray();
            return missingVariants.Length > 0
                ? throw EntityNotFoundException.ByKeys<TaskVariant>(missingVariants)
                : taskVariants.Select(_policyMapper.Map).ToArray();
        }

        public async Task<CheckFailPolicy> FindCheckFailPolicyAsync(Guid checkVariantId)
        {
            var predicate = FilterByCheckVariantId(checkVariantId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .Select(variant => new
                        {
                            variant.FailResultType,
                            variant.FailResult,
                            variant.FailResultCondition
                        });
            var checkFailPolicy = await query.FirstOrDefaultAsync();

            return checkFailPolicy?.FailResultType == null
                ? null
                : _policyMapper.Map(checkFailPolicy.FailResultType.Value,
                                    checkFailPolicy.FailResult,
                                    checkFailPolicy.FailResultCondition);
        }

        private static IQueryable<VerificationPolicy> QueryPolicy(DbContext dbContext, Expression<Func<VerificationPolicy, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .Include(verificationPolicy => verificationPolicy.Tasks)
                   .ThenInclude(policyTask => policyTask.TaskVariant)
                   .ThenInclude(task => task.ChecksVariants);
        }

        private static IQueryable<TaskVariant> QueryTaskVariant(DbContext dbContext, Expression<Func<TaskVariant, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .Include(taskVariant => taskVariant.ChecksVariants);
        }

        private static Expression<Func<VerificationPolicy, bool>> FilterBySelectionContext(VerificationPolicySelectionContext selectionContext)
        {
            if (selectionContext == null)
                throw new ArgumentNullException(nameof(selectionContext));

            Expression<Func<VerificationPolicy, bool>> expression;
            if (selectionContext.State != null)
            {
                expression = policy => policy.RegionType == RegionType.State && policy.Region == selectionContext.State ||
                                       policy.RegionType == RegionType.Country && policy.Region == selectionContext.Country ||
                                       policy.RegionType == RegionType.Region && policy.Region == selectionContext.Region ||
                                       policy.RegionType == RegionType.Global;
            }
            else
            {
                expression = policy => policy.RegionType == RegionType.Country && policy.Region == selectionContext.Country ||
                                       policy.RegionType == RegionType.Region && policy.Region == selectionContext.Region ||
                                       policy.RegionType == RegionType.Global;
            }

            return expression;
        }

        private static Expression<Func<VerificationPolicy, bool>> FilterByPolicyId(Guid policyId) =>
            policy => policy.Id == policyId;

        private static Expression<Func<VerificationPolicy, bool>> FilterByPolicyIds(IEnumerable<Guid> policyIds)
        {
            if (policyIds == null)
                throw new ArgumentNullException(nameof(policyIds));

            return policy => policyIds.Contains(policy.Id);
        }

        private static Expression<Func<TaskVariant, bool>> FilterByTaskVariantId(Guid taskVariantId)
            => taskVariant => taskVariant.Id == taskVariantId;

        private static Expression<Func<TaskVariant, bool>> FilterByTaskVariantIds(Guid[] taskVariantIds)
        {
            if (taskVariantIds == null)
                throw new ArgumentNullException(nameof(taskVariantIds));

            return taskVariant => taskVariantIds.Contains(taskVariant.Id);
        }

        private static Expression<Func<PolicyCheckVariant, bool>> FilterByCheckVariantId(Guid checkVariantId)
             => checkVariant => checkVariant.Id == checkVariantId;

        private static Expression<Func<PolicyCheckVariant, bool>> FilterByCheckVariantIds(Guid[] checkVariantIds)
        {
            if (checkVariantIds == null)
                throw new ArgumentNullException(nameof(checkVariantIds));

            return checkVariant => checkVariantIds.Contains(checkVariant.Id);
        }
    }
}
