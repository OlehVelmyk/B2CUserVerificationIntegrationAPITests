using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps
{
    internal interface IProfileDataExistenceProvider : IBatchJobDataProvider<ProfileDataExistence, CollectionStepsJobSettings> { }

    internal class ProfileDataExistenceProvider : IProfileDataExistenceProvider
    {
        private string SelectPart =
            @$"app.UserId, 
            app.PolicyId,
            app.State,
            case when pd.FirstName is not null AND pd.LastName is not null then 1 else 0 end as {nameof(ProfileDataExistence.FullName)}, 
            case when pd.DateOfBirth is not null then 1 else 0 end as {nameof(ProfileDataExistence.DateOfBirth)},
            case when ad.UserId is not null then 1 else 0 end as {nameof(ProfileDataExistence.Address)},
            case when vd.IpAddress is not null then 1 else 0 end as {nameof(ProfileDataExistence.IpAddress)},
            case when vd.TaxResidence is not null then 1 else 0 end as {nameof(ProfileDataExistence.TaxResidence)},
            case when vd.IdDocumentNumber is not null then 1 else 0 end as {nameof(ProfileDataExistence.IdDocumentNumber)}, 
            case when vd.Tin is not null then 1 else 0 end as {nameof(ProfileDataExistence.Tin)},
            case when vd.Nationality is not null then 1 else 0 end as {nameof(ProfileDataExistence.Nationality)}";

        private readonly IQueryFactory _queryFactory;
        private readonly ICsvBlobStorage _csvBlobStorage;

        public ProfileDataExistenceProvider(IQueryFactory queryFactory, ICsvBlobStorage csvBlobStorage)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async Task<int> GetTotalCountAsync(CollectionStepsJobSettings settings, CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            return users.Length;
        }

        public async IAsyncEnumerable<ICollection<ProfileDataExistence>> GetAsync(CollectionStepsJobSettings settings,
                                                                                  [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (users.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var usersInBatch = users.Skip(page * size).Take(size).ToArray();
                using var factory = _queryFactory.Create();
                var query = BuildQuery(factory, usersInBatch);
                var batch = await factory.GetAsync<ProfileDataExistence>(query, cancellationToken: cancellationToken);
                yield return batch.ToArray();
            }
        }

        private Query BuildQuery(QueryFactory factory, Guid[] usersInBatch) =>
            factory.Query("Applications as app")
                   .SelectRaw(SelectPart)
                   .LeftJoin("PersonalDetails as pd", "pd.UserId", "app.UserId")
                   .LeftJoin("ResidenceAddresses as ad", "ad.UserId", "app.UserId")
                   .LeftJoin("VerificationDetails as vd", "vd.UserId", "app.UserId")
                   .WhereInIfNotEmpty("app.UserId", usersInBatch)
                   .OrderBy("app.Id");

        private async Task<Guid[]> GetUsers(CollectionStepsJobSettings settings)
        {
            if (!settings.Users.IsNullOrEmpty())
                return settings.Users;

            var users = await _csvBlobStorage.GetAsync<Models.User>(settings.ContainerName, settings.FileName);
            return users.Select(user => user.UserId).ToArray();
        }
    }
}