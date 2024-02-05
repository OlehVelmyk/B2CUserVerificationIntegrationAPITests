using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    internal interface IApplicantDataProvider : IBatchJobDataProvider<ApplicantData, SelfieJobSettings>
    {
    }

    internal class ApplicantDataProvider : IApplicantDataProvider
    {
        private readonly IQueryFactory _queryFactory;
        private ICsvBlobStorage _csvBlobStorage;

        public ApplicantDataProvider(IQueryFactory queryFactory, ICsvBlobStorage csvBlobStorage)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async IAsyncEnumerable<ICollection<ApplicantData>> GetAsync(SelfieJobSettings settings, CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (users.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var usersInBatch = users.Skip(page * size).Take(size).ToArray();
                var factory = _queryFactory.Create();
                var query = BuildQuery(usersInBatch, factory);
                var batch = await factory.GetAsync<ApplicantData>(query, cancellationToken: cancellationToken);
                yield return usersInBatch.GroupJoin(batch,
                                                    userId => userId,
                                                    data => data.UserId,
                                                    (userId, externalProfile) => new ApplicantData
                                                    {
                                                        UserId = userId,
                                                        Id = externalProfile.FirstOrDefault()?.Id
                                                    })
                                         .ToArray();
            }
        }

        public async Task<int> GetTotalCountAsync(SelfieJobSettings settings, CancellationToken cancellationToken) =>
            (await GetUsers(settings)).Length;

        private static Query BuildQuery(Guid[] userIds, QueryFactory db) =>
            db.Query("ExternalProfiles")
              .Select("UserId", "ExternalId AS Id")
              .WhereUserIdIn(userIds)
              .Where<ExternalProviderType>("Provider", ExternalProviderType.Onfido)
              .OrderBy("UserId");

        private async Task<Guid[]> GetUsers(SelfieJobSettings settings)
        {
            if (!settings.Users.IsNullOrEmpty())
                return settings.Users;

            var users = await _csvBlobStorage.GetAsync<Worker.Jobs.Models.User>(settings.ContainerName, settings.FileName);
            return users.Select(user => user.UserId).ToArray();
        }
    }
}
