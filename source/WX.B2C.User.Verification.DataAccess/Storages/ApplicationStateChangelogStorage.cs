using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ApplicationStateChangelogStorage : IApplicationStateChangelogStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IApplicationStateChangelogMapper _mapper;

        public ApplicationStateChangelogStorage(IDbContextFactory dbContextFactory, IApplicationStateChangelogMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApplicationStateChangelogDto> FindAsync(Guid applicationId)
        {
            var predicate = FilterByApplicationId(applicationId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var applicationStateChangelog = await query.FirstOrDefaultAsync();

            return applicationStateChangelog == null ? null : _mapper.Map(applicationStateChangelog);
        }

        private static Expression<Func<Entities.ApplicationStateChangelog, bool>> FilterByApplicationId(Guid applicationId) =>
            applicationStateChangelog => applicationStateChangelog.ApplicationId == applicationId;
    }
}