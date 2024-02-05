using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IApplicationDataProvider : IBatchJobDataProvider<ApplicationData, UserBatchJobSettings> { }

    internal class ApplicationDataProvider : IApplicationDataProvider
    {
        private readonly IQueryFactory _queryFactory;

        public ApplicationDataProvider(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<ApplicationData>> GetAsync(UserBatchJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.PaginateAsync<ApplicationData>(factory => factory.Query("Applications")
                                                                             .Select("Id", "UserId", "PolicyId")
                                                                             .WhereUserIdIn(settings.Users)
                                                                             .OrderBy("Id"), settings, cancellationToken);

        public Task<int> GetTotalCountAsync(UserBatchJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.CountAsync(factory => factory.Query("Applications")
                                                         .WhereUserIdIn(settings.Users), cancellationToken);
    }
}