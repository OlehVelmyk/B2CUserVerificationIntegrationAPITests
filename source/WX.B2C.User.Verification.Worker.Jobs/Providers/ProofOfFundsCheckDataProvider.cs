using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IProofOfFundsCheckDataProvider : IBatchJobDataProvider<ProofOfFundsChecksData, UserBatchJobSettings>
    {
    }

    internal class ProofOfFundsCheckDataProvider : IProofOfFundsCheckDataProvider
    {
        private readonly IUserVerificationKeyVault _userVerificationKeyVault;

        public ProofOfFundsCheckDataProvider(IUserVerificationKeyVault userVerificationKeyVault)
        {
            _userVerificationKeyVault = userVerificationKeyVault ?? throw new ArgumentNullException(nameof(userVerificationKeyVault));
        }

        /// <summary>
        /// TODO code must be rewritten
        /// </summary>

        public IAsyncEnumerable<ICollection<ProofOfFundsChecksData>> GetAsync(UserBatchJobSettings settings,
                                                                                   [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //var page = 0;
            //var itemsInBatch = 0;
            //var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();
            //var usersPredicate = settings.Users.AndIn("UserId");
            //// Find users with ongoing checks
            //var query =
            //    @$"SELECT DISTINCT UserId FROM ProofOfFundsChecks 
            //       WHERE Status <= 2
            //       {usersPredicate}
            //       ORDER BY UserId
            //       OFFSET {BatchQuery.OffsetParameter} ROWS FETCH NEXT {BatchQuery.BatchSizeParameter} ROWS ONLY";

            //var command = new BatchQuery(query, settings.ReadingBatchSize);
            //do
            //{
            //    await using var dbConnection = new SqlConnection(connectionString);
            //    var batch = (await dbConnection.QueryAsync<Guid>(command.ForPage(page))).ToArray();
            //    itemsInBatch = batch.Length;
            //    page++;
            //    yield return Map(batch, true);
            //} while (itemsInBatch == settings.ReadingBatchSize &&
            //         !cancellationToken.IsCancellationRequested);

            //page = 0;
            //// Find users without ongoing checks
            //query =
            //    @$"SELECT UserId FROM ProofOfFundsChecks
            //       {usersPredicate}
            //       HAVING MIN(Status) >= 3
            //       ORDER BY UserId
            //       OFFSET {BatchQuery.OffsetParameter} ROWS FETCH NEXT {BatchQuery.BatchSizeParameter} ROWS ONLY";

            //command = new BatchQuery(query, settings.ReadingBatchSize);
            //do
            //{
            //    await using var dbConnection = new SqlConnection(connectionString);
            //    var batch = (await dbConnection.QueryAsync<Guid>(command.ForPage(page))).ToArray();
            //    itemsInBatch = batch.Length;
            //    page++;
            //    yield return Map(batch, false);
            //} while (itemsInBatch == settings.ReadingBatchSize &&
            //         !cancellationToken.IsCancellationRequested);
        }

        public async Task<int> GetTotalCountAsync(UserBatchJobSettings settings, CancellationToken cancellationToken)
        {
            var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();
            var usersPredicate = settings.Users.WhereUserIdIn();
            var query = $@"SELECT COUNT(DISTINCT UserId) 
                           FROM ProofOfFundsChecks
                           {usersPredicate}";

            await using var dbConnection = new SqlConnection(connectionString);
            return await dbConnection.ExecuteScalarAsync<int>(query);
        }

        private static ICollection<ProofOfFundsChecksData> Map(IEnumerable<Guid> usersIds, bool hasOngoingChecks) =>
            usersIds.Select(userId => new ProofOfFundsChecksData { UserId = userId, HasOngoing = hasOngoingChecks }).ToArray();
    }
}