using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class ApplicationStateChangelogRepository : IApplicationStateChangelogRepository
    {
        private readonly IApplicationStateChangelogMapper _mapper;
        private readonly IDbContextFactory _dbContextFactory;

        public ApplicationStateChangelogRepository(
            IApplicationStateChangelogMapper mapper,
            IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApplicationStateChangelogDto> FindAsync(Guid applicationId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, applicationId);
            return _mapper.Map(entity);
        }

        public async Task SaveAsync(ApplicationStateChangelogDto applicationStateChangelog)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, applicationStateChangelog.ApplicationId);

            if (entity == null)
                dbContext.Add(_mapper.MapToEntity(applicationStateChangelog));
            else
            {
                _mapper.Update(entity, applicationStateChangelog);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<Entities.ApplicationStateChangelog> FindAsync(DbContext dbContext, Guid applicationId)
        {
            var query = dbContext
                   .Set<Entities.ApplicationStateChangelog>()
                   .Where(application => application.ApplicationId == applicationId);

            return query.SingleOrDefaultAsync();
        }
    }
}