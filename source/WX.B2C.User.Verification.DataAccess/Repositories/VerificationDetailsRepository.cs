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
    internal class VerificationDetailsRepository : IVerificationDetailsRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IVerificationDetailsMapper _mapper;

        public VerificationDetailsRepository(IDbContextFactory dbContextFactory, IVerificationDetailsMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<VerificationDetailsDto> FindAsync(Guid userId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, userId);

            return entity == null ? null : _mapper.Map(entity);
        }

        public async Task SaveAsync(VerificationDetailsDto verificationDetailsDto)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, verificationDetailsDto.UserId);

            if (entity == null)
            {
                var verificationDetails = _mapper.Map(verificationDetailsDto);
                dbContext.Add(verificationDetails);
            }
            else
            {
                _mapper.Update(verificationDetailsDto, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<VerificationDetails> FindAsync(DbContext dbContext, Guid userId)
        {
            var query = dbContext
                        .Set<VerificationDetails>()
                        .Where(details => details.UserId == userId);

            return query.SingleOrDefaultAsync();
        }
    }
}