using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    internal interface ITaskCreatingDataProvider : IBatchJobDataProvider<TasksCreatingData, TaskCreatingJobSettings>
    { }

    internal class TaskCreatingDataProvider : ITaskCreatingDataProvider
    {
        private readonly IQueryFactory _dbQueryFactory;
        private readonly ICsvBlobStorage _csvBlobStorage;
        private readonly ITaskCreatingDataAggregationService _jobDataAggregationService;

        public TaskCreatingDataProvider(IQueryFactory dbQueryFactory,
                                        ICsvBlobStorage csvBlobStorage,
                                        ITaskCreatingDataAggregationService jobDataAggregationService)
        {
            _dbQueryFactory = dbQueryFactory ?? throw new ArgumentNullException(nameof(dbQueryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
            _jobDataAggregationService = jobDataAggregationService ?? throw new ArgumentNullException(nameof(jobDataAggregationService));
        }

        public async IAsyncEnumerable<ICollection<TasksCreatingData>> GetAsync(TaskCreatingJobSettings settings, CancellationToken cancellationToken)
        {
            var csvData = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);

            var size = settings.ReadingBatchSize;
            var pageCount = (csvData.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var csvBatch = csvData.Skip(page * size).Take(size).ToArray();
                var dbBatch = await QueryAsync(csvBatch, settings, cancellationToken);
                var jobDataBatch = _jobDataAggregationService.Aggregate(dbBatch, settings);
                yield return jobDataBatch;
            }
        }

        public async Task<int> GetTotalCountAsync(TaskCreatingJobSettings settings, CancellationToken cancellationToken)
        {
            var csvData = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);
            return csvData.Length;
        }

        private async Task<IEnumerable<DbData>> QueryAsync(IEnumerable<CsvData> csvBatch, TaskCreatingJobSettings settings, CancellationToken cancellationToken)
        {
            using var factory = _dbQueryFactory.Create();

            var userIds = csvBatch.Select(csv => csv.UserId);
            var query =
                new Query("Applications as A")
                      .SelectRaw("A.UserId, A.Id as ApplicationId, A.PolicyId, V.Type as TaskType")
                      .LeftJoin("ApplicationTasks as AT", "A.Id", "AT.ApplicationId")
                      .LeftJoin("VerificationTasks as V", j => j.On("AT.TaskId", "V.Id")
                                                                .WhereEnums("V.Type", settings.TaskTypes))
                      .WhereIn("A.UserId", userIds);

            return await factory.GetAsync<DbData>(query, cancellationToken: cancellationToken);
        }

        // TODO: Make it private, untie this class from tests 
        internal class CsvData
        {
            public Guid UserId { get; set; }
        }

        internal class DbData
        {
            public Guid UserId { get; set; }

            public Guid ApplicationId { get; set; }

            public Guid PolicyId { get; set; }

            public TaskType? TaskType { get; set; }
        }
    }
}
