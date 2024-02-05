using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps
{
    internal interface ICreateStepStateJobProvider : IBatchJobDataProvider<UserTasks, CreateStepStateJobSettings> { }

    internal class CreateStepStateJobProvider : ICreateStepStateJobProvider
    {
        private readonly ICsvBlobStorage _csvBlobStorage;
        private readonly IQueryFactory _queryFactory;

        public CreateStepStateJobProvider(ICsvBlobStorage csvBlobStorage, IQueryFactory queryFactory)
        {
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public async Task<int> GetTotalCountAsync(CreateStepStateJobSettings settings, CancellationToken cancellationToken) =>
            (await GetUsers(settings)).Length;

        public async IAsyncEnumerable<ICollection<UserTasks>> GetAsync(CreateStepStateJobSettings settings,
                                                                       CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (users.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var usersInBatch = users.Skip(page * size).Take(size).ToArray();
                using var factory = _queryFactory.Create();
                var query = BuildQuery(usersInBatch, settings.TaskTypes);
                var batch = await factory.GetAsync<DbData>(query, cancellationToken: cancellationToken);
                yield return usersInBatch.GroupJoin(batch,
                                                    userId => userId,
                                                    data => data.UserId,
                                                    CreateUserTasks)
                                         .ToArray();
            }
        }

        private Query BuildQuery(Guid[] usersInBatch, TaskType[] taskTypes) =>
            new Query("VerificationTasks").Select("Id", "UserId", "Type")
                                          .WhereIn("UserId", usersInBatch)
                                          .WhereInEnums("Type", taskTypes);

        private async Task<Guid[]> GetUsers(CreateStepStateJobSettings settings)
        {
            if (!settings.Users.IsNullOrEmpty())
                return settings.Users;

            var users = await _csvBlobStorage.GetAsync<Worker.Jobs.Models.User>(settings.ContainerName, settings.FileName);
            return users.Select(user => user.UserId).ToArray();
        }

        private static UserTasks CreateUserTasks(Guid userId, IEnumerable<DbData> tasks) =>
            new()
            {
                UserId = userId,
                Tasks = tasks.Select(data => new UserTasks.Task
                             {
                                 Id = data.Id,
                                 Type = data.Type,
                             })
                             .ToArray()
            };
        
        
        private class DbData
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public TaskType Type { get; set; }
        }
    }
}