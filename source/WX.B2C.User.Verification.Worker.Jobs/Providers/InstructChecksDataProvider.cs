using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using SqlKata;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IInstructChecksDataProvider : IBatchJobDataProvider<UserInstructChecksData, InstructChecksJobSettings>
    { }

    internal class InstructChecksDataProvider : IInstructChecksDataProvider
    {
        private readonly IQueryFactory _queryFactory;
        private readonly ICsvBlobStorage _csvBlobStorage;

        public InstructChecksDataProvider(IQueryFactory queryFactory, ICsvBlobStorage csvBlobStorage)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async IAsyncEnumerable<ICollection<UserInstructChecksData>> GetAsync(InstructChecksJobSettings settings, CancellationToken cancellationToken)
        {
            var csvData = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);
            csvData = csvData.Where(data => !data.AcceptanceChecks.IsNullOrEmpty())
                             .GroupBy(data => new { data.UserId, data.TaskType })
                             .Select(grouping => new CsvData
                             {
                                 UserId = grouping.Key.UserId, TaskType = grouping.Key.TaskType,
                                 AcceptanceChecks = grouping.SelectMany(data => data.AcceptanceChecks).Distinct().ToArray()
                             }).ToArray();
            
            var groupedCsvData = csvData.GroupBy(item => item.UserId).ToArray();

            var size = settings.ReadingBatchSize;
            var pageCount = (csvData.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var csvBatch = groupedCsvData.Skip(page * size).Take(size).ToArray();

                using var factory = _queryFactory.Create();

                var queryApplications = new Query("Applications")
                                        .Select($"Applications.Id as {nameof(Application.ApplicationId)}",
                                                "Applications.UserId")
                                        .WhereIn("Applications.UserId", csvBatch.Select(g => g.Key));
                var applications = (await factory.GetAsync<Application>(queryApplications, cancellationToken: cancellationToken)).ToArray();
                var usersWithApplications = applications.Select(application => application.UserId);
                
                var query = new Query("VerificationTasks")
                            .Select("VerificationTasks.Id", "VerificationTasks.UserId", "VerificationTasks.Type", "Checks.VariantId")
                            .LeftJoin("TaskChecks", "VerificationTasks.Id", "TaskChecks.TaskId")
                            .LeftJoin("Checks", "TaskChecks.CheckId", "Checks.Id")
                            .WhereIn("VerificationTasks.UserId", usersWithApplications);

                var dbTasks = await factory.GetAsync<Task>(query, cancellationToken: cancellationToken);

                var tasks = dbTasks.GroupBy(data => new { Id = data.Id, data.UserId, data.Type })
                                   .Select(group => new VerificationTask
                                   {
                                       TaskId = group.Key.Id,
                                       UserId = group.Key.UserId,
                                       Type = group.Key.Type,
                                       Checks = group.Where(data => data.VariantId != null)
                                                     .Select(data => data.VariantId.Value)
                                                     .ToArray()
                                   });

                var jobDataBatch = csvBatch.GroupJoin(tasks,
                                                      csv => csv.Key,
                                                      db => db.UserId,
                                                      (csv, dbData) => new UserInstructChecksData
                                                      {
                                                          UserId = csv.Key,
                                                          TaskAcceptanceChecks = TaskSelector(csv, dbData)
                                                      });
                
                jobDataBatch = jobDataBatch.GroupJoin(applications,
                                                      data => data.UserId,
                                                      application => application.UserId,
                                                      (data, application) =>
                                                      {
                                                          data.ApplicationId = application.Select(app => (Guid?)app.ApplicationId).FirstOrDefault();
                                                          return data;
                                                      });
                
                yield return jobDataBatch.OrderBy(data => data.UserId).ToArray();
            }
        }

        private TaskAcceptanceChecksData[] TaskSelector(IGrouping<Guid, CsvData> group, IEnumerable<VerificationTask> tasks) =>
            group.GroupJoin(tasks,
                            csv => csv.TaskType,
                            task => task.Type,
                            (csv, t) => new TaskAcceptanceChecksData
                            {
                                TaskType = csv.TaskType,
                                NewChecks = csv.AcceptanceChecks,
                                Tasks = t.Select(db => new TaskAcceptanceChecksData.TaskData
                                {
                                    Id = db.TaskId,
                                    ExitingChecks = db.Checks
                                }).ToArray()
                            }).ToArray();
        
        public async Task<int> GetTotalCountAsync(InstructChecksJobSettings settings, CancellationToken cancellationToken)
        {
            var data = await _csvBlobStorage.GetAsync<CsvData>(settings.ContainerName, settings.FileName);
            var grouped = data.GroupBy(item => item.UserId).ToArray();
            return grouped.Length;
        }

        // TODO: Make it private, untie this class from tests 
        internal class CsvData
        {
            public Guid UserId { get; set; }

            public TaskType TaskType { get; set; }

            public Guid[] AcceptanceChecks { get; set; }
        }

        private class Application
        {
            public Guid ApplicationId { get; set; }

            public Guid UserId { get; set; }
        }
        
        private class Task
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public TaskType Type { get; set; }

            public Guid? VariantId { get; set; }
        }
        
        private class VerificationTask
        {
            public Guid TaskId { get; set; }

            public Guid UserId { get; set; }

            public TaskType Type { get; set; }

            public Guid[] Checks { get; set; }
        }
    }
}
