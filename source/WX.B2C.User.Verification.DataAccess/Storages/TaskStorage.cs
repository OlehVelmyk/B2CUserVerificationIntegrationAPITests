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
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Extensions;
using TaskType = WX.B2C.User.Verification.Domain.Models.TaskType;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class TaskStorage : ITaskStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ITaskMapper _mapper;

        public TaskStorage(IDbContextFactory dbContextFactory, ITaskMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TaskDto[]> FindByCheckIdAsync(Guid checkId)
        {
            var predicate = FilterByCheckId(checkId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var tasks = await query.ToArrayAsync();

            return tasks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<TaskDto[]> FindByStepIdAsync(Guid collectionStepId)
        {
            var predicate = FilterByCollectionStepId(collectionStepId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var tasks = await query.ToArrayAsync();

            return tasks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<TaskDto[]> GetAllAsync(Guid userId, TaskType? taskType = null)
        {
            var predicate = FilterByUserId(userId);
            if (taskType.HasValue)
                predicate = predicate.And(FilterByTaskType(taskType.Value));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var tasks = await query.ToArrayAsync();

            return tasks.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<TaskDto> FindAsync(Guid taskId, Guid userId)
        {
            var predicate = FilterById(taskId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var task = await query.FirstOrDefaultAsync();

            return task?.UserId != userId ? null : _mapper.MapToDto(task);
        }

        public async Task<TaskDto> GetAsync(Guid taskId)
        {
            var predicate = FilterById(taskId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var task = await query.FirstOrDefaultAsync();

            return task == null
                ? throw EntityNotFoundException.ByKey<VerificationTask>(taskId)
                : _mapper.MapToDto(task);
        }

        public async Task<TaskDto[]> GetByApplicationIdAsync(Guid applicationId)
        {
            var predicate = FilterByApplicationId(applicationId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryApplicationTask(dbContext, predicate);
            var tasks = await query.ToArrayAsync();

            return tasks.MapToArray(task => _mapper.MapToDto(task.Task));
        }

        public async Task<TaskDto> GetAsync(Guid applicationId, TaskType taskType)
        {
            var task = await FindAsync(applicationId, taskType);
            return task ?? throw EntityNotFoundException.ByQuery<ApplicationTask>(new { applicationId, taskType });
        }

        public async Task<TaskDto> FindAsync(Guid applicationId, TaskType taskType)
        {
            var predicate = FilterByApplicationId(applicationId).And(FilterByApplicationTaskType(taskType));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryApplicationTask(dbContext, predicate);
            var task = await query.FirstOrDefaultAsync();

            return task == null ? null : _mapper.MapToDto(task.Task);
        }

        public async Task<Domain.Models.VerificationTask[]> FindAsync(Guid userId, IEnumerable<Guid> variantIds)
        {
            var predicate = FilterByUserId(userId).And(FilterByVariantIds(variantIds));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryVerificationTask(dbContext, predicate);
            var tasks = await query.ToArrayAsync();

            return tasks.Select(_mapper.Map).ToArray();
        }

        public async Task<TaskPriorityGroupDto[]> GetPriorityGroupsAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var variantsQuery = dbContext.Set<TaskVariant>().AsNoTracking();
            var query = QueryVerificationTask(dbContext, predicate)
                .Join(variantsQuery,
                    task => task.VariantId,
                    variant => variant.Id,
                    (task, variant) => new
                    {
                        task.Id,
                        task.VariantId,
                        task.State,
                        variant.Priority,
                        Checks = task.PerformedChecks
                                     .Select(check => check.CheckId)
                                     .ToArray()
                    });

            var items = await query.ToArrayAsync();
            
            return items.GroupBy(x => x.Priority)
                        .Select(x => new TaskPriorityGroupDto
                        {
                            Priority = x.Key,
                            Checks = x.SelectMany(task => task.Checks).ToArray(),
                            Tasks = x.Select(task => new PriorityTaskDto
                            {
                                Id = task.Id,
                                VariantId = task.VariantId,
                                State = task.State
                            }).ToArray(),
                        }).ToArray();
        }

        private static IQueryable<VerificationTask> QueryVerificationTask(
            DbContext dbContext, Expression<Func<VerificationTask, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .Include(x => x.PerformedChecks).ThenInclude(x => x.Check)
                   .Include(x => x.CollectionSteps).ThenInclude(x => x.Step);
        }

        private static IQueryable<ApplicationTask> QueryApplicationTask(
            DbContext dbContext, Expression<Func<ApplicationTask, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .Include(x => x.Task)
                   .ThenInclude(x => x.PerformedChecks)
                   .ThenInclude(x => x.Check)
                   .Include(x => x.Task)
                   .ThenInclude(x => x.CollectionSteps)
                   .ThenInclude(x => x.Step);
        }

        private static Expression<Func<VerificationTask, bool>> FilterById(Guid id) =>
            task => task.Id == id;

        private static Expression<Func<VerificationTask, bool>> FilterByUserId(Guid userId) =>
            task => task.UserId == userId;

        private static Expression<Func<VerificationTask, bool>> FilterByCheckId(Guid checkId) =>
            task => task.PerformedChecks.Any(check => check.CheckId == checkId);

        private static Expression<Func<VerificationTask, bool>> FilterByCollectionStepId(Guid collectionStepId) =>
            task => task.CollectionSteps.Any(check => check.StepId == collectionStepId);

        private static Expression<Func<ApplicationTask, bool>> FilterByApplicationId(Guid applicationId) =>
            applicationTask => applicationTask.ApplicationId == applicationId;

        private static Expression<Func<VerificationTask, bool>> FilterByTaskType(TaskType taskType) =>
            task => task.Type == taskType;

        private static Expression<Func<ApplicationTask, bool>> FilterByApplicationTaskType(TaskType taskType) =>
            applicationTask => applicationTask.Task.Type == taskType;

        private static Expression<Func<VerificationTask, bool>> FilterByVariantIds(IEnumerable<Guid> variantIds)
        {
            if (variantIds == null)
                throw new ArgumentNullException(nameof(variantIds));

            return task => variantIds.Contains(task.VariantId);
        }
    }
}