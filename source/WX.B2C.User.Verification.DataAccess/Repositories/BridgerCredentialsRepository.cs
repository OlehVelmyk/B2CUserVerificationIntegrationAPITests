using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class BridgerCredentialsRepository : IBridgerCredentialsRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IBridgerCredentialsMapper _mapper;

        public BridgerCredentialsRepository(
            IDbContextFactory dbContextFactory, 
            IBridgerCredentialsMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task SaveAsync(BridgerCredentialsDto credentialsDto)
        {
            var credentials = _mapper.Map(credentialsDto);

            await using var dbContext = _dbContextFactory.Create();
            dbContext.Add(credentials);
            await dbContext.SaveChangesAsync();
        }
    }
}
