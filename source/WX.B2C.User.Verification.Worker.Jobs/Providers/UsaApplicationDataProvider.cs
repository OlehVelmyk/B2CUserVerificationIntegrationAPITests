using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IUsaApplicationDataProvider : IBatchJobDataProvider<ApplicationData, FraudScreeningTaskJobSettings> { }

    internal class UsaApplicationDataProvider : IUsaApplicationDataProvider
    {
        private const string UsaPolicyId = "4B6271BD-FDE5-40F7-8701-29AA66865568";
        private readonly IQueryFactory _queryFactory;

        public UsaApplicationDataProvider(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<ApplicationData>> GetAsync(FraudScreeningTaskJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.PaginateAsync<ApplicationData>(factory => BuildQuery(factory,settings), settings, cancellationToken);

        public Task<int> GetTotalCountAsync(FraudScreeningTaskJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.CountAsync(factory => BuildQuery(factory, settings), cancellationToken);

        private Query BuildQuery(QueryFactory factory, FraudScreeningTaskJobSettings settings) =>
            factory.Query("Applications")
                   .Select("Id", "UserId")
                   .WhereUserIdIn(settings.Users)
                   .Where("PolicyId", UsaPolicyId)
                   .OrderBy("UserId");
    }
}