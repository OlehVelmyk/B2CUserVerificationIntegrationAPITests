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
    internal interface IProofOfAddressProvider : IBatchJobDataProvider<ProofOfAddressData, ProofOfAddressJobSetting>
    {
    }

    internal class ProofOfAddressProvider : IProofOfAddressProvider
    {
        private readonly IQueryFactory _queryFactory;

        public ProofOfAddressProvider(IUserVerificationQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<ProofOfAddressData>> GetAsync(ProofOfAddressJobSetting settings, CancellationToken cancellationToken)
        {
            var lastPoaCheck =
                new Query("ProofOfAddressChecks")
                    .Select("ProofOfAddressChecks.*")
                    .LeftJoin("ProofOfAddressChecks as b",
                              join => join.On("ProofOfAddressChecks.UserId", "b.UserId")
                                          .On("ProofOfAddressChecks.CreatedAt", "b.CreatedAt", "<"))
                    .WhereNull("b.CreatedAt");

            Query BuildQuery(QueryFactory factory) =>
                factory.Query("ProfileInformations").
                        Select("ProfileInformationId AS UserId", "LastPoaCheck.Status", "IsCountryMatchedByIp").
                        LeftJoin(lastPoaCheck.As("LastPoaCheck"), join => @join.On("LastPoaCheck.UserId", "ProfileInformations.ProfileInformationId")).
                        WhereInIfNotEmpty("ProfileInformationId", settings.Users).
                        OrderBy("UserId");

            return _queryFactory.PaginateAsync<ProofOfAddressData>(BuildQuery, settings, cancellationToken);
        }

        public Task<int> GetTotalCountAsync(ProofOfAddressJobSetting settings, CancellationToken cancellationToken) =>
            _queryFactory.Create()
                         .Query("ProfileInformations")
                         .WhereInIfNotEmpty("ProfileInformationId", settings.Users)
                         .CountAsync<int>(cancellationToken: cancellationToken);
    }
}