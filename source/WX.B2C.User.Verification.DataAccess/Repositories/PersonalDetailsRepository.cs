using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class PersonalDetailsRepository : IPersonalDetailsRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IPersonalDetailsMapper _mapper;

        public PersonalDetailsRepository(IDbContextFactory dbContextFactory, IPersonalDetailsMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PersonalDetailsDto> FindAsync(Guid userId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, userId);

            return entity == null ? null : _mapper.Map(entity);
        }

        public async Task SaveAsync(PersonalDetailsDto dto)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, dto.UserId);
            
            if (entity == null)
            {
                entity = _mapper.Map(dto);
                dbContext.Add(entity);
            }
            else
            {
                _mapper.Update(dto, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<PersonalDetails> FindAsync(DbContext dbContext, Guid userId)
        {
            var query = dbContext
                        .Set<PersonalDetails>()
                        .Include(details => details.ResidenceAddress)
                        .Where(details => details.UserId == userId);

            return query.SingleOrDefaultAsync();
        }
    }
}