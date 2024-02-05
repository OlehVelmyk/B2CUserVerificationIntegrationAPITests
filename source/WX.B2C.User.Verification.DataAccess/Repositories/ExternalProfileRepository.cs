using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class ExternalProfileRepository : IExternalProfileRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IExternalProfileMapper _mapper;

        public ExternalProfileRepository(IDbContextFactory dbContextFactory, IExternalProfileMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ExternalProfileDto> FindAsync(Guid userId, ExternalProviderType providerType)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, userId, providerType);

            return entity == null ? null : _mapper.Map(entity);
        }

        public async Task SaveAsync(Guid userId, ExternalProfileDto externalProfileDto)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, userId, externalProfileDto.Provider);

            if (entity == null)
                dbContext.Add(_mapper.Map(userId, externalProfileDto));
            else
            {
                _mapper.Update(externalProfileDto, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<ExternalProfile> FindAsync(DbContext dbContext, Guid userId, ExternalProviderType providerType)
        {
            var query = dbContext
                        .Set<ExternalProfile>()
                        .Where(info => info.UserId == userId && info.Provider == providerType);

            return query.SingleOrDefaultAsync();
        }
    }
}