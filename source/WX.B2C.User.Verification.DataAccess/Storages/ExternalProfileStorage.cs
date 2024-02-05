using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ExternalProfileStorage : IExternalProfileStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IExternalProfileMapper _mapper;

        public ExternalProfileStorage(IDbContextFactory dbContextFactory, IExternalProfileMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ExternalProfileDto> FindAsync(Guid userId, ExternalProviderType providerType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProvider(providerType));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var externalProfile = await query.FirstOrDefaultAsync();

            return externalProfile == null ? null : _mapper.Map(externalProfile);
        }

        public async Task<string> GetExternalIdAsync(Guid userId, ExternalProviderType externalProviderType)
        {
            var externalId = await FindExternalIdAsync(userId, externalProviderType);
            return externalId ?? throw EntityNotFoundException.ByQuery<ExternalProfile>(new { userId, externalProviderType });
        }

        public async Task<string> FindExternalIdAsync(Guid userId, ExternalProviderType externalProviderType)
        {
            var predicate = FilterByUserId(userId).And(FilterByProvider(externalProviderType));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var externalProfile = await query.FirstOrDefaultAsync();

            return externalProfile?.ExternalId;
        }

        public async Task<ExternalProfileDto[]> FindAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var externalProfiles = await query.ToArrayAsync();

            return externalProfiles.Select(_mapper.Map).ToArray();
        }

        private static Expression<Func<ExternalProfile, bool>> FilterByUserId(Guid userId) =>
            profile => profile.UserId == userId;

        private static Expression<Func<ExternalProfile, bool>> FilterByProvider(ExternalProviderType providerType) =>
            profile => profile.Provider == providerType;
    }
}
