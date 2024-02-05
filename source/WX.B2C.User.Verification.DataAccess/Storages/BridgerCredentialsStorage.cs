using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class BridgerCredentialsStorage : IBridgerCredentialsStorage
    {
        private readonly IDbContextFactory _dbContextFactory;

        public BridgerCredentialsStorage(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<string> GetPasswordAsync(string userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(credentials => credentials.CreatedAt)
                        .Select(x => x.EncryptedPassword);
            var encryptedPassword = await query.FirstOrDefaultAsync();

            return encryptedPassword ?? throw EntityNotFoundException.ByKey<BridgerCredentials>(userId);
        }

        private static Expression<Func<BridgerCredentials, bool>> FilterByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return credentials => credentials.UserId == userId;
        }
    }
}
