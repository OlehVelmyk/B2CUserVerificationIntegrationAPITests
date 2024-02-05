using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SqlKata.Execution;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels;
using WX.B2C.User.Verification.Worker.Jobs.Services;

using static WX.B2C.User.Verification.Extensions.TaskExtensions;
using Check = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels.Check;
using CollectionStep = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels.CollectionStep;
using EnumerableExtensions = WX.B2C.User.Verification.Extensions.EnumerableExtensions;
using Task = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels.Task;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects
{
    internal interface IUserDefectsModelProvider : IBatchJobDataProvider<UserConsistency, DetectDefectJobSettings> { }

    internal class UserDefectsModelProvider : IUserDefectsModelProvider
    {
        private readonly ICsvBlobStorage _csvBlobStorage;
        private readonly IQueryFactory _queryFactory;

        public UserDefectsModelProvider(ICsvBlobStorage csvBlobStorage, IQueryFactory queryFactory)
        {
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public async Task<int> GetTotalCountAsync(DetectDefectJobSettings settings, CancellationToken cancellationToken)
        {
            if (settings.UserPredicate != null)
            {
                using var factory = _queryFactory.Create();
                var query = factory.Query("Applications").WhereRaw(settings.UserPredicate).AsCount();
                return await factory.ExecuteScalarAsync<int>(query, cancellationToken: cancellationToken);
            }
            
            return (await GetUsersFromSettings(settings)).Length;
        }

        public async IAsyncEnumerable<ICollection<UserConsistency>> GetAsync(DetectDefectJobSettings settings,
                                                                             [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var usersInBatch in GetUsersAsync(settings, cancellationToken))
            {
                var (profileData, documents, profiles, tasks, checks, steps) =
                    await WhenAll(GetProfileData(usersInBatch),
                                  GetDocuments(usersInBatch),
                                  GetExternalProfiles(usersInBatch),
                                  GetTasks(usersInBatch),
                                  GetChecks(usersInBatch),
                                  GetCollectionSteps(usersInBatch));

                var models = usersInBatch.GroupJoin(profileData,
                                                    userId => userId,
                                                    profile => profile.UserId,
                                                    (userId, entities) => new UserConsistency
                                                    {
                                                        UserId = userId,
                                                    }.With(entities.FirstOrDefault()));

                yield return models
                             .GroupJoin(documents, user => user.UserId, entity => entity.UserId, (user, entities) => user.With(entities))
                             .GroupJoin(profiles, user => user.UserId, entity => entity.UserId, (user, dbModels) => user.With(dbModels))
                             .GroupJoin(tasks, user => user.UserId, entity => entity.UserId, (user, dbModels) => user.With(dbModels))
                             .GroupJoin(checks, user => user.UserId, entity => entity.UserId, (user, dbModels) => user.With(dbModels))
                             .GroupJoin(steps, user => user.UserId, entity => entity.UserId, (user, dbModels) => user.With(dbModels))
                             .ToArray();
            }
        }

        private async IAsyncEnumerable<Guid[]> GetUsersAsync(DetectDefectJobSettings settings, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (settings.UserPredicate != null)
            {
                using var factory = _queryFactory.Create();
                var query = factory.Query("Applications").WhereRaw(settings.UserPredicate).Select("UserId").OrderBy("UserId");
                var paginate = await factory.PaginateAsync<Guid>(query, 1, perPage: settings.ReadingBatchSize, cancellationToken: cancellationToken);
                foreach (var paginationResult in paginate.Each)
                {
                    yield return paginationResult.List.ToArray();
                }
            }
            else
            {
                var users = await GetUsersFromSettings(settings);
                var size = settings.ReadingBatchSize;
                var pageCount = (users.Length - 1) / size + 1;
                for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
                {
                    var usersInBatch = users.Skip(page * size).Take(size).ToArray();
                    yield return usersInBatch;
                }
            }
        }

        private async Task<Guid[]> GetUsersFromSettings(DetectDefectJobSettings settings)
        {
            if (!EnumerableExtensions.IsNullOrEmpty(settings.Users))
                return settings.Users;

            var users = await _csvBlobStorage.GetAsync<Worker.Jobs.Models.User>(settings.ContainerName, settings.FileName);
            return users.Select(user => user.UserId).ToArray();
        }

        private async Task<IEnumerable<Profile>> GetProfileData(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var SelectPart = @$"
            app.UserId, 
            app.Id as ApplicationId,
            app.PolicyId,
            app.State as ApplicationState,
            case when pd.FirstName is not null AND pd.LastName is not null then 1 else 0 end as {nameof(Profile.FullName)}, 
            case when pd.DateOfBirth is not null then 1 else 0 end as {nameof(Profile.DateOfBirth)},
            case when ad.UserId is not null then 1 else 0 end as {nameof(Profile.Address)},
            case when vd.IpAddress is not null then 1 else 0 end as {nameof(Profile.IpAddress)},
            case when vd.TaxResidence is not null then 1 else 0 end as {nameof(Profile.TaxResidence)},
            case when vd.IdDocumentNumber is not null then 1 else 0 end as {nameof(Profile.IdDocumentNumber)}, 
            case when vd.IdDocumentNumberType is not null then 1 else 0 end as {nameof(Profile.IdDocumentNumberType)}, 
            case when vd.Tin is not null then 1 else 0 end as {nameof(Profile.Tin)},
            case when vd.Nationality is not null then 1 else 0 end as {nameof(Profile.Nationality)},
            case when vd.RiskLevel is not null then 1 else 0 end as {nameof(Profile.RiskLevel)}";

            var query = factory.Query("Applications as app")
                               .SelectRaw(SelectPart)
                               .LeftJoin("PersonalDetails as pd", "pd.UserId", "app.UserId")
                               .LeftJoin("ResidenceAddresses as ad", "ad.UserId", "app.UserId")
                               .LeftJoin("VerificationDetails as vd", "vd.UserId", "app.UserId")
                               .WhereInIfNotEmpty("app.UserId", usersInBatch)
                               .OrderBy("app.Id");

            return await query.GetAsync<Profile>();
        }

        private async Task<IEnumerable<Document>> GetDocuments(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var query = factory.Query("Documents")
                               .Select<Document>()
                               .GroupBy("UserId", "Category")
                               .WhereInIfNotEmpty("UserId", usersInBatch);
            return await query.GetAsync<Document>();
        }

        private async Task<IEnumerable<ExternalProfile>> GetExternalProfiles(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var query = factory.Query("ExternalProfiles")
                               .Select<ExternalProfile>()
                               .WhereInIfNotEmpty("UserId", usersInBatch);
            return await query.GetAsync<ExternalProfile>();
        }

        private async Task<IEnumerable<Task>> GetTasks(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var query = factory.Query("VerificationTasks")
                               .Select<Task>()
                               .LeftJoin("ApplicationTasks", "TaskId", "Id")
                               .WhereInIfNotEmpty("UserId", usersInBatch);
            return await query.GetAsync<Task>();
        }

        private async Task<IEnumerable<Check>> GetChecks(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var query = factory.Query("Checks")
                               .Select<Check>()
                               .LeftJoin("TaskChecks", "CheckId", "Id")
                               .WhereInIfNotEmpty("UserId", usersInBatch);
            return await query.GetAsync<Check>();
        }

        private async Task<IEnumerable<CollectionStep>> GetCollectionSteps(Guid[] usersInBatch)
        {
            using var factory = _queryFactory.Create();

            var query = factory.Query("CollectionSteps")
                               .Select<CollectionStep>()
                               .LeftJoin("TaskCollectionSteps", "StepId", "Id")
                               .WhereInIfNotEmpty("UserId", usersInBatch);
            return await query.GetAsync<CollectionStep>();
        }
    }
}