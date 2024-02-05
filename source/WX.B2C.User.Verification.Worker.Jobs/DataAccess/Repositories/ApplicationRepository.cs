using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories
{
    internal interface IApplicationRepository
    {
        Task LinkTasksAsync(Guid applicationId, Guid[] tasks);
    }

    internal class ApplicationRepository : IApplicationRepository
    {
        private const string ApplicationTasks = nameof(ApplicationTasks);
        private const string ApplicationId = nameof(ApplicationId);
        private const string TaskId = nameof(TaskId);

        private readonly IQueryFactory _queryFactory;

        public ApplicationRepository(IQueryFactory dbQueryFactory)
        {
            _queryFactory = dbQueryFactory ?? throw new ArgumentNullException(nameof(dbQueryFactory));
        }

        public async Task LinkTasksAsync(Guid applicationId, Guid[] tasks)
        {
            if (tasks.IsNullOrEmpty())
                return;

            using var factory = _queryFactory.Create();

            var cols = new[] { ApplicationId, TaskId };
            var data = tasks.Select(taskId => new object[] { applicationId, taskId }).ToArray();

            var query = factory.Query(ApplicationTasks)
                               .AsInsert(cols, data);

            await factory.ExecuteAsync(query);

            //TODO if will run not in maintenance, make sense to create post check if was added task with type which already added due to race condition 
        }
    }
}