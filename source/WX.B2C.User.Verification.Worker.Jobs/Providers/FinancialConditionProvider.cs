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
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IFinancialConditionProvider : IBatchJobDataProvider<SurveyCheckData, FinancialConditionJobSetting>
    {
    }

    internal class FinancialConditionProvider : IFinancialConditionProvider
    {
        private readonly IUserVerificationKeyVault _userVerificationKeyVault;

        public FinancialConditionProvider(IUserVerificationKeyVault userVerificationKeyVault)
        {
            _userVerificationKeyVault = userVerificationKeyVault ?? throw new ArgumentNullException(nameof(userVerificationKeyVault));
        }

        public async IAsyncEnumerable<ICollection<SurveyCheckData>> GetAsync(FinancialConditionJobSetting settings, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var page = 0;
            var itemsInBatch = 0;

            var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();
            var sql = @"
                       SELECT Id, UserId, Status, Result, SurveyId 
                       FROM(SELECT Id, UserId, Status, Result, SurveyId, row_number = ROW_NUMBER() over(PARTITION BY UserId, Type ORDER BY CreatedAt DESC) FROM SurveyChecks WHERE Type = 5) as lastSurveyPerUser
                       WHERE row_number = 1 " + WhereUsers(settings) + WhereSurveys(settings) +
                       "ORDER BY Id DESC OFFSET @offset ROWS FETCH NEXT @batchSize ROWS ONLY";

            var query = new BatchQuery(sql, settings.ReadingBatchSize);

            do
            {
                await using var dbConnection = new SqlConnection(connectionString);

                var batch = (await dbConnection.QueryAsync<SurveyCheckData>(query.ForPage(page++))).ToArray();
                itemsInBatch = batch.Length;
                yield return batch;
            } while (itemsInBatch == settings.ReadingBatchSize && cancellationToken.IsCancellationRequested);
        }

        public async Task<int> GetTotalCountAsync(FinancialConditionJobSetting settings, CancellationToken cancellationToken)
        {
            var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();

            var query = @"SELECT COUNT(1)  
                        FROM(SELECT Id, UserId, Status, Result, SurveyId, row_number = ROW_NUMBER() over(PARTITION BY UserId, Type ORDER BY CreatedAt DESC) FROM SurveyChecks WHERE Type = 5) as lastSurveyPerUser 
                        WHERE row_number = 1 " + WhereUsers(settings) + WhereSurveys(settings);

            await using var dbConnection = new SqlConnection(connectionString);
            return await dbConnection.ExecuteScalarAsync<int>(query);
        }

        private static string WhereUsers(FinancialConditionJobSetting settings)
        {
            var whereUsers = settings.Users != null && settings.Users.Any()
                ? $"AND UserId IN ({string.Join(',', settings.Users.Select(id => $"'{id}'").ToArray())}) "
                : string.Empty;
            return whereUsers;
        }

        private static string WhereSurveys(FinancialConditionJobSetting settings)
        {
            var whereSurveys = settings.SurveyIds != null && settings.SurveyIds.Any()
                ? $"AND SurveyId IN ({string.Join(',', settings.SurveyIds.Select(id => $"'{id}'").ToArray())}) "
                : string.Empty;
            return whereSurveys;
        }
    }
}