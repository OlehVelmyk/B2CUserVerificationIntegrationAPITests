using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class MonitoringPolicyStorage : IMonitoringPolicyStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IMonitoringPolicyMapper _mapper;

        public MonitoringPolicyStorage(IDbContextFactory dbContextFactory, IMonitoringPolicyMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MonitoringPolicyDto> FindAsync(MonitoringPolicySelectionContext selectionContext)
        {
            var predicate = BuildPredicate(selectionContext);

            await using var dbContext = _dbContextFactory.Create();
            var query = Query(dbContext, predicate);
            var policy = await query.FirstOrDefaultAsync();
            return policy == null ? null : _mapper.Map(policy);
        }

        private static IQueryable<MonitoringPolicy> Query(DbContext dbContext, Expression<Func<MonitoringPolicy, bool>> predicate) =>
            dbContext.QueryAsNoTracking<MonitoringPolicy>()
                     .Where(predicate)
                     .OrderByDescending(verificationPolicy => verificationPolicy.RegionType);

        private static Expression<Func<MonitoringPolicy, bool>> BuildPredicate(MonitoringPolicySelectionContext policySelectionContext)
        {
            if (policySelectionContext == null)
                throw new ArgumentNullException(nameof(policySelectionContext));

            return policy => policy.RegionType == RegionType.Country && policy.Region == policySelectionContext.Country ||
                             policy.RegionType == RegionType.Region && policy.Region == policySelectionContext.Region ||
                             policy.RegionType == RegionType.Global;
        }
    }
}