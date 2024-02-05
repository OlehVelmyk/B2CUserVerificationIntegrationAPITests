using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories
{
    internal interface ITaskCollectionStepRepository
    {
        Task AddNewCollectionStepsAsync(Guid taskId, Guid[] steps);

        Task LinkAsync(IEnumerable<(Guid TaskId, Guid StepId)> taskSteps);
    }

    internal class TaskCollectionStepRepository : ITaskCollectionStepRepository
    {
        private const string TaskCollectionStepsTableName = "TaskCollectionSteps";
        private const string StepIdColumnName = "StepId";
        private const string TaskIdColumnName = "TaskId";

        private readonly IQueryFactory _queryFactory;

        public TaskCollectionStepRepository(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public async Task AddNewCollectionStepsAsync(Guid taskId, Guid[] steps)
        {
            using var factory = _queryFactory.Create();

            var selectExisting = new Query(TaskCollectionStepsTableName).Select(StepIdColumnName).Where(TaskIdColumnName, taskId);
            var existing = await factory.GetAsync<Guid>(selectExisting);
            var data = steps.Except(existing).Select(stepId => new object[] { taskId, stepId }).ToArray();
            if (data.Length == 0)
                return;

            var columns = new[] { TaskIdColumnName, StepIdColumnName };
            var insertQuery = new Query(TaskCollectionStepsTableName).AsInsert(columns, data);
            await factory.ExecuteAsync(insertQuery);
        }

        public async Task LinkAsync(IEnumerable<(Guid TaskId, Guid StepId)> taskSteps)
        {
            var data = taskSteps.Select(tuple => new object[] { tuple.TaskId, tuple.StepId }).ToArray();
            if (data.Length == 0)
                return;

            using var factory = _queryFactory.Create();

            var columns = new[] { TaskIdColumnName, StepIdColumnName };
            var insertQuery = new Query(TaskCollectionStepsTableName).AsInsert(columns, data);
            await factory.ExecuteAsync(insertQuery);
        }
    }
}