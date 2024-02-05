using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class CheckProviderConfigurationStorage : ICheckProviderConfigurationStorage
    {
        private readonly IDbContextFactory _dbContextFactory;

        public CheckProviderConfigurationStorage(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<CheckProviderConfigurationDto> GetAsync(Guid checkVariantId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking<PolicyCheckVariant>()
                        .Where(x => x.Id == checkVariantId)
                        .Select(x => new CheckProviderConfigurationDto
                        {
                            CheckType = x.Type,
                            ProviderType = x.Provider,
                            Config = x.Config
                        });
            var configuration = await query.FirstOrDefaultAsync();
            return configuration ?? throw EntityNotFoundException.ByKey<PolicyCheckVariant>(checkVariantId);
        }
    }
}