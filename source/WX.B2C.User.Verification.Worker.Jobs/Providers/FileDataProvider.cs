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
    internal interface IFileDataProvider : IBatchJobDataProvider<FileData, FileValidationJobSettings> { }

    internal class FileDataProvider : IFileDataProvider
    {
        private readonly IQueryFactory _queryFactory;

        public FileDataProvider(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public IAsyncEnumerable<ICollection<FileData>> GetAsync(FileValidationJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.PaginateAsync<FileData>(factory => BuildQuery(factory, settings), settings, cancellationToken);

        public Task<int> GetTotalCountAsync(FileValidationJobSettings settings, CancellationToken cancellationToken) =>
            _queryFactory.CountAsync(factory => BuildQuery(factory, settings), cancellationToken);

        private static Query BuildQuery(QueryFactory factory, FileValidationJobSettings settings) =>
            factory.Query("Files")
                   .WhereUserIdIn(settings.Users)
                   .WhereInIfNotEmpty("Id", settings.Files)
                   .OrderBy("Id");
    }
}