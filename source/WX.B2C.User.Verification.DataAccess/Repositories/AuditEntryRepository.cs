using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class AuditEntryRepository : IAuditEntryRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAuditEntityMapper _mapper;

        public AuditEntryRepository(
            IDbContextFactory dbContextFactory, 
            IAuditEntityMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task SaveAsync(AuditEntryDto auditDto)
        {
            var auditEntry = _mapper.Map(auditDto);

            await using var dbContext = _dbContextFactory.Create();
            dbContext.Add(auditEntry);
            await dbContext.SaveChangesAsync();
        }
    }
}