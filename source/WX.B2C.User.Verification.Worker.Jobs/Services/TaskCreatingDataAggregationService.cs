using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using DbData = WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks.TaskCreatingDataProvider.DbData;
using TaskVariantData = WX.B2C.User.Verification.Worker.Jobs.Models.TasksCreatingData.TaskVariantData;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal interface ITaskCreatingDataAggregationService : IJobDataAggregationService<DbData, TaskCreatingJobSettings, TasksCreatingData>
    { }

    internal class TaskCreatingDataAggregationService : ITaskCreatingDataAggregationService
    {
        private readonly ITaskVariantProvider _taskVariantProvider;
        private readonly ILogger _logger;

        public TaskCreatingDataAggregationService(ITaskVariantProvider taskVariantProvider, ILogger logger)
        {
            _taskVariantProvider = taskVariantProvider ?? throw new ArgumentNullException(nameof(taskVariantProvider));
            _logger = logger?.ForContext<TaskCreatingDataAggregationService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public TasksCreatingData[] Aggregate(IEnumerable<DbData> batch, TaskCreatingJobSettings settings)
        {
            var tasksCreatingData = new List<TasksCreatingData>();

            var requestedTasks = settings.TaskTypes.Distinct().ToArray();
            foreach (var group in batch.GroupBy(db => db.UserId))
            {
                var data = TasksCreatingDataSelector(group, requestedTasks);
                tasksCreatingData.Add(data);
            }

            return tasksCreatingData.ToArray();
        }

        private TasksCreatingData TasksCreatingDataSelector(IGrouping<Guid, DbData> group, TaskType[] requestedTasks)
        {
            var (userId, policyId, appId) = group.Select(db => (db.UserId, db.PolicyId, db.ApplicationId)).First();

            var createdTasks = group.Where(db => db.TaskType.HasValue).Select(db => db.TaskType).ToArray();
            var taskVariants = new List<TaskVariantData>();
            foreach (var taskType in requestedTasks)
            {
                var taskVariant = _taskVariantProvider.Find(policyId, taskType);
                if (taskVariant is null)
                {
                    _logger.Warning("Not found task variant for task type {TaskType} in policy {PolicyId}. UserId {UserId}",
                                    taskType,
                                    policyId,
                                    userId);
                    continue;
                }

                var data = new TaskVariantData
                {
                    Id = taskVariant.Id,
                    Type = taskType,
                    AlreadyCreated = createdTasks.Contains(taskType)
                };
                taskVariants.Add(data);
            }

            return new TasksCreatingData
            {
                UserId = userId,
                ApplicationId = appId,
                TaskVariants = taskVariants.ToArray()
            };
        }
    }
}
