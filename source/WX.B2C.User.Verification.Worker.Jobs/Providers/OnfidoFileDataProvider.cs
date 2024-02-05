using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Execution;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface IOnfidoFileDataProvider : IBatchJobDataProvider<OnfidoFileData, OnfidoDocumentOcrJobSetting> { }

    internal class OnfidoFileDataProvider : IOnfidoFileDataProvider
    {
        private readonly IQueryFactory _queryFactory;

        public OnfidoFileDataProvider(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<OnfidoFileData>> GetAsync(OnfidoDocumentOcrJobSetting settings,
                                                                      CancellationToken cancellationToken) =>
            _queryFactory.PaginateAsync<OnfidoFileData>(factory => BuildQuery(factory, settings), settings, cancellationToken);

        public Task<int> GetTotalCountAsync(OnfidoDocumentOcrJobSetting settings,
                                            CancellationToken cancellationToken) =>
            _queryFactory.CountAsync(factory => BuildQuery(factory, settings), cancellationToken);

        private static Query BuildQuery(QueryFactory factory, OnfidoDocumentOcrJobSetting settings) =>
            factory.Query("Files")
                   .WhereUserIdIn(settings.Users)
                   .WhereInIfNotEmpty("Id", settings.Files)
                   .Where<ExternalProviderType>("Provider", ExternalProviderType.Onfido)
                   .OrderBy("Id");
    }
}