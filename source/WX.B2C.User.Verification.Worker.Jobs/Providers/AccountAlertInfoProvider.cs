using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IAccountAlertInfoProvider : IBatchJobDataProvider<AccountAlertInfo, BatchJobSettings> { }

    internal class AccountAlertInfoProvider : IAccountAlertInfoProvider
    {
        private readonly AccountAlertJobConfig _jobConfig;
        private readonly IQueryFactory _queryFactory;

        public AccountAlertInfoProvider(AccountAlertJobConfig jobConfig, IQueryFactory queryFactory)
        {
            _jobConfig = jobConfig ?? throw new ArgumentNullException(nameof(jobConfig));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<AccountAlertInfo>> GetAsync(BatchJobSettings settings, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.Date;
            return _queryFactory.PaginateAsync<AccountAlertInfo>(factory => BuildQuery(factory, currentDate), settings, cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(BatchJobSettings settings, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.Date;
            using var queryFactory = _queryFactory.Create();
            var query = BuildQuery(queryFactory, currentDate);

            return await query.CountAsync<int>(cancellationToken: cancellationToken);
        }

        private Query BuildQuery(QueryFactory queryFactory, DateTime currentDate)
        {
            var query = queryFactory
                        .Query("Applications as A")
                        .SelectRaw("A.UserId, A.State AS ApplicationState, C.LastApprovedDate, V.RiskLevel, V.Turnover")
                        .Join("ApplicationStateChangelog as C", "C.ApplicationId", "A.Id")
                        .Join("VerificationDetails as V", "V.UserId", "A.UserId")
                        .Join("ResidenceAddresses as R", "R.UserId", "A.UserId")
                        .Where("A.State", _jobConfig.ApplicationState.ToString())
                        .WhereNotIn("R.Country", _jobConfig.ExcludedCountries);

            return query.WhereRaw(BuildPeriodPredicate(currentDate))
                        .OrderBy("A.Id");
        }

        /// <summary>
        /// TODO use SQL kata for building phase 2
        /// </summary>
        private string BuildPeriodPredicate(DateTime currentDate)
        {
            var predicates = _jobConfig.Periods
                                       .Select(period => PredicateBuilder
                                                         .With(BuildRiskLevelPredicate(period.RiskLevel))
                                                         .And(period.OverallTurnover.ColumnMoreOrEquals("V.Turnover"))
                                                         .And(BuildApprovingDatePredicate(currentDate.AddYears(-period.AccountAge)))
                                                         .Build())
                                       .Aggregate(PredicateBuilder.Empty(),
                                                  (builder, predicate, index) =>
                                                      index == 0 ? PredicateBuilder.With(predicate) : builder.Or(predicate))
                                       .Build();

            return $"({predicates})";

            string BuildRiskLevelPredicate(RiskLevel riskLevel) =>
                riskLevel is RiskLevel.Low ? $"(V.RiskLevel = '{RiskLevel.Low}' OR V.RiskLevel is null)" : $"V.RiskLevel = '{riskLevel}'";

            string BuildApprovingDatePredicate(DateTime approvingDate) =>
                $"DATEDIFF(day,C.LastApprovedDate,CONVERT(DATETIME,'{approvingDate.ToString(CultureInfo.InvariantCulture)}', 101)) = 0";
        }
    }
}