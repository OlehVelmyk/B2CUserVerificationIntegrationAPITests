using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ApplicationStorage : IApplicationStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IApplicationMapper _mapper;

        public ApplicationStorage(IDbContextFactory dbContextFactory, IApplicationMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApplicationState> GetStateAsync(Guid userId, ProductType productType)
        {
            var state = await FindStateAsync(userId, productType);
            return state ?? throw EntityNotFoundException.ByQuery<Application>(new { userId, productType });
        }

        public async Task<ApplicationState?> FindStateAsync(Guid userId, ProductType productType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProductType(productType));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(application => application.CreatedAt)
                        .Select(application => (ApplicationState?)application.State);
            var state = await query.FirstOrDefaultAsync();

            return state;
        }

        public async Task<ApplicationDto> GetAsync(Guid applicationId)
        {
            var predicate = FilterByApplicationId(applicationId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var application = await query.FirstOrDefaultAsync();

            return application == null
                ? throw EntityNotFoundException.ByKey<Application>(applicationId)
                : _mapper.MapToDto(application);
        }

        public async Task<ApplicationDto> FindAsync(Guid userId, Guid applicationId)
        {
            var predicate = FilterByApplicationId(applicationId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var application = await query.FirstOrDefaultAsync();

            return application?.UserId != userId ? null : _mapper.MapToDto(application);
        }

        public async Task<ApplicationDto[]> FindAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var applications = await query.ToArrayAsync();

            return applications.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<ApplicationDto[]> FindAsync(Guid userId, ApplicationState applicationState)
        {
            var predicate = FilterByUserId(userId).And(FilterByApplicationState(applicationState));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var applications = await query.ToArrayAsync();

            return applications.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<ApplicationDto> FindAsync(Guid userId, ProductType productType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProductType(productType));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var application = await query.FirstOrDefaultAsync();

            return application == null ? null : _mapper.MapToDto(application);
        }

        public async Task<Guid?> FindIdAsync(Guid userId, ProductType productType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProductType(productType));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(application => application.CreatedAt)
                        .Select(application => (Guid?)application.Id);
            var applicationId = await query.FirstOrDefaultAsync();

            return applicationId;
        }

        public async Task<ApplicationDto[]> FindAssociatedWithTaskAsync(Guid taskId)
        {
            var predicate = FilterByTaskId(taskId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithTasks(dbContext, predicate);
            var applications = await query.ToArrayAsync();

            return applications.Select(_mapper.MapToDto).ToArray();
        }

        public async Task<bool> IsAutomatedAsync(Guid userId, ProductType productType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProductType(productType));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(application => application.CreatedAt)
                        .Select(application => application.IsAutomating);

            // Assumed that if application doesn't exists automation is false by default.
            // Can be changed to Option or nullable bool, but then everywhere need to be do the same: isAutomated.ValueOrDefault()
            var isAutomated = await query.FirstOrDefaultAsync();

            return isAutomated;
        }

        private static IQueryable<Entities.Application> QueryWithTasks(
            DbContext dbContext, Expression<Func<Entities.Application, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .Include(application => application.RequiredTasks)
                   .ThenInclude(applicationTask => applicationTask.Task)
                   .OrderByDescending(application => application.CreatedAt);
        }

        private static Expression<Func<Entities.Application, bool>> FilterByUserId(Guid userId) =>
            application => application.UserId == userId;

        private static Expression<Func<Entities.Application, bool>> FilterByApplicationId(Guid applicationId) =>
            application => application.Id == applicationId;

        private static Expression<Func<Entities.Application, bool>> FilterByTaskId(Guid taskId) =>
            application => application.RequiredTasks.Any(x => x.Task.Id == taskId);

        private static Expression<Func<Entities.Application, bool>> FilterByProductType(ProductType productType) =>
            application => application.ProductType == productType;

        private static Expression<Func<Entities.Application, bool>> FilterByApplicationState(ApplicationState applicationState) =>
            application => application.State == applicationState;

        private static Expression<Func<Entities.Application, bool>> FilterNotRejected() =>
            application => application.State != ApplicationState.Rejected && application.State != ApplicationState.Cancelled;
    }
}
