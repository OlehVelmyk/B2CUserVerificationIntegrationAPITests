using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IChecksDataProvider : IBatchJobDataProvider<ReRunCheckJobData, RerunChecksJobSettings> { }

    internal class ChecksDataProvider : IChecksDataProvider
    {
        private const int MaxParametersCount = 2000;

        private readonly IQueryFactory _queryFactory;
        private readonly ICsvBlobStorage _csvBlobStorage;
        private readonly ILogger _logger;

        public ChecksDataProvider(IQueryFactory queryFactory, ICsvBlobStorage csvBlobStorage, ILogger logger)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
            _logger = logger?.ForContext<ChecksDataProvider>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> GetTotalCountAsync(RerunChecksJobSettings settings, CancellationToken cancellationToken)
        {
            var checks = await GetChecksAsync(settings);
            var factory = _queryFactory.Create();
            var size = MaxParametersCount;
            var pageCount = (checks.Length - 1) / size + 1;
            var users = new HashSet<Guid>();

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var checksInBatch = checks.Skip(page * size).Take(size).ToArray();
                var query = factory.Query("Checks")
                                   .SelectRaw("distinct(UserId) as UserId")
                                   .WhereIn("Id", checksInBatch);
                var usersInBatch = await factory.GetAsync<Guid>(query, cancellationToken: cancellationToken);
                usersInBatch.Foreach(id => users.Add(id));
            }

            return users.Count;
        }

        public async IAsyncEnumerable<ICollection<ReRunCheckJobData>> GetAsync(RerunChecksJobSettings settings,
                                                                               [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var checksIds = await GetChecksAsync(settings);
            var size = MaxParametersCount;
            var pageCount = (checksIds.Length - 1) / size + 1;

            _logger.Information("Start reading all checks to re-run");
            var checks = new List<DbData>();
            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var checksInBatch = checksIds.Skip(page * size).Take(size).ToArray();
                var factory = _queryFactory.Create();
                var query = BuildQuery(checksInBatch, factory);
                var batch = await factory.GetAsync<DbData>(query, cancellationToken: cancellationToken);
                checks.AddRange(batch);
                _logger.Information("Read {Page} of {PageCount}", page, pageCount);
            }

            _logger.Information("Start mapping checks to re-run");
            var userChecks = Map(checks);
            _logger.Information("Finish mapping all checks to re-run");

            size = settings.ReadingBatchSize;
            pageCount = (userChecks.Length - 1) / size + 1;
            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                yield return userChecks.Skip(page * size).Take(size).ToArray();
            }
        }

        private static ReRunCheckJobData[] Map(IEnumerable<DbData> checks) =>
            checks.GroupBy(data => data.UserId,
                           (userId, userChecks) => new ReRunCheckJobData
                           {
                               UserId = userId,
                               ChecksToRerun = userChecks.Select(data => new CheckData
                                                         {
                                                             CheckId = data.Id,
                                                             VariantId = data.VariantId,
                                                             Provider = data.Provider,
                                                             Type = data.Type,
                                                             State = data.State,
                                                             //TODO later we can have one to many, need to be changed logic
                                                             RelatedTasks = new[] { data.TaskId }
                                                         })
                                                         .ToArray()
                           }).ToArray();

        private static Query BuildQuery(Guid[] checksIds, QueryFactory db) =>
            db.Query("Checks")
              .Select<DbData>()
              .Join("TaskChecks", "TaskChecks.CheckId", nameof(DbData.Id))
              .WhereIn(nameof(DbData.Id), checksIds);

        private async Task<Guid[]> GetChecksAsync(RerunChecksJobSettings settings)
        {
            if (!settings.Checks.IsNullOrEmpty())
                return settings.Checks;

            var checks = await _csvBlobStorage.GetAsync<EntityIdentifier>(settings.ContainerName, settings.FileName);
            return checks.Select(check => check.Id).ToArray();
        }

        private class DbData
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public CheckType Type { get; set; }

            public Guid VariantId { get; set; }

            public CheckProviderType Provider { get; set; }

            public CheckState State { get; set; }

            public Guid TaskId { get; set; }
        }
    }
}