using System;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.DataAccess.EF;

namespace WX.B2C.User.Verification.DataAccess
{
    public interface IDbContextFactory
    {
        DbContext Create();
    }

    internal class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions _databaseOptions;

        public DbContextFactory(DbContextOptions databaseOptions)
        {
            _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        public DbContext Create() => new VerificationDbContext(_databaseOptions);
    }
}
