using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories
{
    internal interface ITaskRepository
    {
        Task<Guid> CreateAsync(TaskTemplate taskTemplate);

        Task CompleteAsync(Guid taskId, TaskResult newResult);

        Task IncompleteAsync(Guid taskId);

        Task AddTaskChecksAsync(Guid taskId, Guid[] instructedChecks);

        Task AddTasksChecksAsync(IEnumerable<(Guid TaskId, Guid CheckId)> tasksChecks);
    }

    internal class TaskRepository : ITaskRepository
    {
        private const string TaskTableName = "VerificationTasks";
        private const string TaskChecksTableName = "TaskChecks";
        private const string TaskIdColumnName = "Id";

        private readonly IQueryFactory _queryFactory;

        public TaskRepository(IQueryFactory dbQueryFactory)
        {
            _queryFactory = dbQueryFactory ?? throw new ArgumentNullException(nameof(dbQueryFactory));
        }

        public async Task<Guid> CreateAsync(TaskTemplate taskTemplate)
        {
            using var factory = _queryFactory.Create();

            var id = Guid.NewGuid();
            var insert = factory.Query(TaskTableName)
                                .InsertAsync(new
                                {
                                    Id = id,
                                    UserId = taskTemplate.UserId,
                                    Type = taskTemplate.Type.ToString(),
                                    VariantId = taskTemplate.VariantId,
                                    CreationDate = DateTime.UtcNow,
                                    State = TaskState.Incomplete.ToString(),
                                    AcceptanceCheckIds = "[]",
                                    IsExpired = false,

                                });

            await insert;

            return id;
        }

        public Task AddTaskChecksAsync(Guid taskId, Guid[] instructedChecks) =>
            AddTasksChecksAsync(instructedChecks.Select(checkId => (TaskId: taskId, CheckId: checkId)));

        public async Task AddTasksChecksAsync(IEnumerable<(Guid TaskId, Guid CheckId)> tasksChecks)
        {
            using var factory = _queryFactory.Create();
            var columns = new[]
            {
                "TaskId",
                "CheckId"
            };
            var rows = tasksChecks.Select(tuple => new object[] { tuple.TaskId, tuple.CheckId }).ToArray();

            var insertQuery = new Query(TaskChecksTableName)
                .AsInsert(columns, rows);

            await factory.ExecuteAsync(insertQuery);
        }

        public async Task CompleteAsync(Guid taskId, TaskResult newResult)
        {
            using var factory = _queryFactory.Create();

            var updateQuery = factory.Query(TaskTableName)
                                     .Where(TaskIdColumnName, taskId)
                                     .UpdateAsync(new
                                     {
                                         State = TaskState.Completed.ToString(),
                                         Result = newResult.ToString()
                                     });

            await updateQuery;
        }


        public async Task IncompleteAsync(Guid taskId)
        {
            using var factory = _queryFactory.Create();

            var updateQuery = factory.Query(TaskTableName)
                                     .Where(TaskIdColumnName, taskId)
                                     .UpdateAsync(new
                                     {
                                         State = TaskState.Incomplete.ToString(),
                                         Result = (string) null
                                     });

            await updateQuery;
        }
    }

    internal class TaskTemplate
    {
        public TaskType Type { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }
    }
}
