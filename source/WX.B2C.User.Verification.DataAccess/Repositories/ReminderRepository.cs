using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class ReminderRepository : IReminderRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IReminderMapper _mapper;

        public ReminderRepository(IDbContextFactory dbContextFactory, IReminderMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task SaveAsync(UserReminderDto reminder)
        {
            await using var dbContext = _dbContextFactory.Create();
            dbContext.Add(_mapper.Map(reminder));

            await dbContext.SaveChangesAsync();
        }
    }
}