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
    internal interface ITaxResidenceProvider : IBatchJobDataProvider<TaxResidenceData, TaxResidenceJobSetting> { }

    internal class TaxResidenceProvider : ITaxResidenceProvider
    {
        private readonly IQueryFactory _queryFactory;

        public TaxResidenceProvider(IUserVerificationQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<TaxResidenceData>> GetAsync(TaxResidenceJobSetting settings, CancellationToken cancellationToken) =>
            _queryFactory.PaginateAsync<TaxResidenceData>(factory => BuildQuery(factory, settings),
                                                            settings,
                                                            cancellationToken);

        public Task<int> GetTotalCountAsync(TaxResidenceJobSetting settings, CancellationToken cancellationToken) =>
            _queryFactory.CountAsync(factory => BuildQuery(factory, settings), cancellationToken);

        private static Query BuildQuery(QueryFactory factory, TaxResidenceJobSetting settings) =>
            factory.Query("ProfileInformations")
                   .Select("ProfileInformationId",
                           "VerificationStopReason",
                           "TaxResidencies")
                   .WhereInIfNotEmpty("ProfileInformationId", settings.Users)
                   .OrderBy("ProfileInformationId");
    }
}