using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface ITaskDataProvider : IBatchJobDataProvider<UserTaskData, CsvBlobJobSettings>
    {
    }

    internal class TaskDataProvider : ITaskDataProvider
    {
        private readonly IQueryFactory _queryFactory;
        private readonly ICsvBlobStorage _csvBlobStorage;

        public TaskDataProvider(IQueryFactory queryFactory, ICsvBlobStorage csvBlobStorage)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async Task<int> GetTotalCountAsync(CsvBlobJobSettings settings, CancellationToken cancellationToken)
        {
            var data = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);
            var grouped = data.GroupBy(item => new { item.UserId, item.TaskType }).ToArray();
            return grouped.Length;
        }

        public async IAsyncEnumerable<ICollection<UserTaskData>> GetAsync(CsvBlobJobSettings settings,
                                                                          [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var csvData = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);
            var size = settings.ReadingBatchSize;
            var pageCount = (csvData.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var csvBatch = csvData.Skip(page * size).Take(size).ToArray();

                using var factory = _queryFactory.Create();
                var query = new Query("VerificationTasks").Select("Id", "UserId", "Type", "State", "Result")
                                                          .WhereIn("UserId", csvBatch.Select(g => g.UserId));

                var dbBatch = await factory.GetAsync<DbData>(query);
                var jobDataBatch = csvBatch.GroupJoin(dbBatch,
                                                      csv => (csv.UserId, csv.TaskType),
                                                      db => (db.UserId, db.Type),
                                                      (csv, dbs) => new UserTaskData
                                                      {
                                                          UserId = csv.UserId,
                                                          TaskType = csv.TaskType,
                                                          Tasks = dbs.Select(Map).ToArray()
                                                      })
                                           .OrderBy(data => data.UserId)
                                           .ToArray();
                yield return jobDataBatch;
            }
        }

        private static TaskStateInfo Map(DbData dbData)
        {
            if (dbData == null)
                throw new ArgumentNullException(nameof(dbData));

            return new TaskStateInfo
            {
                Id = dbData.Id,
                State = dbData.State,
                Result = dbData.Result,
                UserId = dbData.UserId
            };
        }

        // TODO: Make it private, untie this class from tests
        internal class CsvData
        {
            public Guid UserId { get; set; }

            public TaskType TaskType { get; set; }
        }

        internal class DbData
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public TaskType Type { get; set; }

            public TaskState State { get; set; }

            public TaskResult? Result { get; set; }
        }
    }
}