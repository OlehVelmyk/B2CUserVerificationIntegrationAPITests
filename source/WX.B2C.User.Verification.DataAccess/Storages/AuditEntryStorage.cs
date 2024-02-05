using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Audit;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class AuditEntryStorage : IAuditEntryStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAuditEntityMapper _mapper;

        public AuditEntryStorage(IDbContextFactory dbContextFactory, IAuditEntityMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedDto<AuditEntryDto>> FindAsync(Guid userId, ODataQueryContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            await using var dbContext = _dbContextFactory.Create();

            var (totalQuery, query) = dbContext
                                      .QueryAsNoTracking<AuditEntry>()
                                      .Where(entry => entry.UserId == userId)
                                      .ApplyOData(context);

            var total = await totalQuery.CountAsync();
            var entries = await query.ToArrayAsync();

            var items = entries.Select(_mapper.MapToDto).ToArray();
            return new PagedDto<AuditEntryDto> { Items = items, Total = total };
        }
    }
}