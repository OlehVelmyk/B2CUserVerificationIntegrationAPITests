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

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class CollectionStepStorage : ICollectionStepStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ICollectionStepMapper _collectionStepMapper;

        public CollectionStepStorage(
            IDbContextFactory dbContextFactory,
            ICollectionStepMapper collectionStepMapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _collectionStepMapper = collectionStepMapper ?? throw new ArgumentNullException(nameof(collectionStepMapper));
        }

        public async Task<CollectionStepDto[]> GetAllAsync(Guid userId)
        {
            var predicate = FilterByUser(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var steps = await query.ToArrayAsync();

            return steps.Select(_collectionStepMapper.MapToDto).ToArray();
        }

        public async Task<CollectionStepDto[]> GetAllAsync(Guid userId, string xPath)
        {
            var predicate = FilterByUser(userId).And(FilterByXPath(new[] { xPath }));;

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var steps = await query.ToArrayAsync();

            return steps.Select(_collectionStepMapper.MapToDto).ToArray();
        }

        public async Task<CollectionStepDto> GetAsync(Guid id)
        {
            var predicate = FilterById(id);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var step = await query.FirstOrDefaultAsync();

            return step == null
                ? throw EntityNotFoundException.ByKey<CollectionStep>(id)
                : _collectionStepMapper.MapToDto(step);
        }

        public async Task<CollectionStepDto> FindAsync(Guid id, Guid userId)
        {
            var predicate = FilterById(id);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var step = await query.FirstOrDefaultAsync();

            return step?.UserId != userId ? null : _collectionStepMapper.MapToDto(step);
        }

        public async Task<CollectionStepDto> FindAsync(Guid userId, string xPath)
        {
            var predicate = FilterByUser(userId)
                .And(FilterByXPath(new[] { xPath }))
                .And(FilterNotCancelled());

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                           .QueryAsNoTracking(predicate)
                           .OrderBy(step => step.State);
            var collectionStep = await query.FirstOrDefaultAsync();
            
            return collectionStep == null ? null : _collectionStepMapper.MapToDto(collectionStep);
        }

        public async Task<CollectionStepDto[]> FindRequestedAsync(Guid userId, params string[] xPathes)
        {
            if (xPathes.IsNullOrEmpty())
                return Array.Empty<CollectionStepDto>();

            var predicate = FilterByUser(userId)
                            .And(FilterRequested())
                            .And(FilterByXPath(xPathes.Distinct()));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var results = await query.ToArrayAsync();

            return results.Select(_collectionStepMapper.MapToDto).ToArray();
        }

        public async Task<CollectionStepDto[]> FindRequestedAsync(Guid userId)
        {
            var predicate = FilterByUser(userId).And(FilterRequested());

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var steps = await query.ToArrayAsync();

            return steps.Select(_collectionStepMapper.MapToDto).ToArray();
        }

        public async Task<Guid[]> GetRelatedTasksAsync(Guid stepId)
        {
            Expression<Func<TaskCollectionStep, bool>> predicate = taskStep => taskStep.StepId == stepId;

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var taskSteps = await query.ToArrayAsync();

            return taskSteps.Select(taskStep => taskStep.TaskId).ToArray();
        }

        private static Expression<Func<CollectionStep, bool>> FilterById(Guid id) =>
            step => step.Id == id;

        private static Expression<Func<CollectionStep, bool>> FilterByUser(Guid userId) =>
            step => step.UserId == userId;

        private static Expression<Func<CollectionStep, bool>> FilterRequested() =>
            step => step.State == Domain.DataCollection.CollectionStepState.Requested;

        private static Expression<Func<CollectionStep, bool>> FilterNotCancelled() =>
            step => step.State != Domain.DataCollection.CollectionStepState.Cancelled;

        private static Expression<Func<CollectionStep, bool>> FilterByXPath(IEnumerable<string> xPathes)
        {
            if (xPathes == null)
                throw new ArgumentNullException(nameof(xPathes));

            return step => xPathes.Contains(step.XPath);
        }
    }
}