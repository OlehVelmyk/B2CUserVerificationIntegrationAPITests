using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps
{
    internal interface IUserSurveyChecksProvider : IBatchJobDataProvider<UserSurveyChecks, CollectionStepsJobSettings> { }

    internal class UserSurveyChecksProvider : IUserSurveyChecksProvider
    {
        private readonly IUserVerificationKeyVault _userVerificationKeyVault;
        private readonly ICsvBlobStorage _csvBlobStorage;

        public UserSurveyChecksProvider(IUserVerificationKeyVault userVerificationKeyVault, ICsvBlobStorage csvBlobStorage)
        {
            _userVerificationKeyVault = userVerificationKeyVault ?? throw new ArgumentNullException(nameof(userVerificationKeyVault));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        /// <summary>
        /// We expect that for prod we prepare a csv file which will have intersection of users with check and migrated applications users.
        /// SELECT distinct(UserId)
        /// FROM SurveyChecks
        /// WHERE Status != 5
        /// Later we can improve the way of calculation count
        /// </summary>
        public async Task<int> GetTotalCountAsync(CollectionStepsJobSettings settings, CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            return users.Length;
        }

        public async IAsyncEnumerable<ICollection<UserSurveyChecks>> GetAsync(CollectionStepsJobSettings settings,
                                                                              [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var users = await GetUsers(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (users.Length - 1) / size + 1;
            var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                var usersInBatch = users.Skip(page * size).Take(size).ToArray();
                var query = BuildQuery(usersInBatch);
                await using var dbConnection = new SqlConnection(connectionString);
                var batch = (await dbConnection.QueryAsync<SurveyCheckEntity>(query)).ToArray();
                var userSurveyChecks = usersInBatch.GroupJoin(batch,
                                                              userId => userId,
                                                              entity => entity.UserId,
                                                              (userId, checks) =>
                                                              {
                                                                  return new UserSurveyChecks
                                                                  {
                                                                      UserId = userId,
                                                                      Checks = checks.Select(entity => new SurveyCheck
                                                                                     {
                                                                                         Type = entity.Type,
                                                                                         Result = entity.Result,
                                                                                         Status = entity.Status
                                                                                     })
                                                                                     .ToArray()
                                                                  };
                                                              });
                yield return userSurveyChecks.ToArray();
            }
        }

        private string BuildQuery(Guid[] usersInBatch)
        {
            var usersPredicate = usersInBatch.AndIn("UserId");
            var query = $@"SELECT 
                           UserId, Type, Status ,Result
                           FROM (
                                SELECT 
                                UserId, Type, Status ,Result,
                                row_number = ROW_NUMBER() over (PARTITION BY UserId,Type ORDER BY CreatedAt DESC)
                                FROM SurveyChecks
                                WHERE Status != 5
                                ) as lastSurveyPerUser
                                WHERE row_number = 1 {usersPredicate}";
            return query;
        }

        private async Task<Guid[]> GetUsers(CollectionStepsJobSettings settings)
        {
            if (!settings.Users.IsNullOrEmpty())
                return settings.Users;

            var users = await _csvBlobStorage.GetAsync<Models.User>(settings.ContainerName, settings.FileName);
            return users.Select(user => user.UserId).ToArray();
        }

        private class SurveyCheckEntity
        {
            public Guid UserId { get; set; }

            public SurveyCheckType Type { get; set; }

            public SurveyCheckStatus Status { get; set; }

            public SurveyCheckResult Result { get; set; }
        }
    }
}