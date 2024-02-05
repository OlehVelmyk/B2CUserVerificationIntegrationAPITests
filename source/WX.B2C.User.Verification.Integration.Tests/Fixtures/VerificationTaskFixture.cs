using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class VerificationTaskFixture
    {
        private readonly DbFixture _dbFixture;

        public VerificationTaskFixture(DbFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        public async Task SaveCollectionSteps(VerificationTask task)
        {
            var collectionSteps = task.CollectionSteps
                                      .Select(step => Map(step, task.UserId))
                                      .ToArray<object>();

            await _dbFixture.DbContext.AddRangeAsync(collectionSteps);
            await _dbFixture.DbContext.SaveChangesAsync();
        }

        public async Task SaveChecks(VerificationTask task)
        {
            var checks = task.Checks
                             .Select(check => Map(check, task.UserId))
                             .ToArray<object>();

            var taskChecks = task.Checks
                                 .Select(check => Map(task.Id, check.Id))
                                 .ToArray<object>();

            await _dbFixture.DbContext.AddRangeAsync(checks);
            await _dbFixture.DbContext.AddRangeAsync(taskChecks);
            await _dbFixture.DbContext.SaveChangesAsync();
        }

        public async Task SaveCollectionStepAsync(TaskCollectionStep taskStep, Guid userId)
        {
            await _dbFixture.DbContext.AddAsync(Map(taskStep, userId));
            await _dbFixture.DbContext.SaveChangesAsync();
        }

        private static Verification.DataAccess.Entities.Check Map(TaskCheck taskCheck, Guid userId) =>
            new()
            {
                Id = taskCheck.Id,
                UserId = userId,
                Type = taskCheck.Type,
                VariantId = taskCheck.VariantId,
                State = taskCheck.State,
                Result = taskCheck.Result,
                Provider = CheckProviderType.System,
            };

        private static Verification.DataAccess.Entities.CollectionStep Map(TaskCollectionStep taskStep, Guid userId) =>
            new()
            {
                Id = taskStep.Id,
                IsRequired = taskStep.IsRequired,
                State = taskStep.State,
                UserId = userId,
                XPath = "test"
            };

        private static Verification.DataAccess.Entities.TaskCheck Map(Guid taskId, Guid checkId) =>
            new()
            {
                CheckId = checkId,
                TaskId = taskId
            };
    }
}
