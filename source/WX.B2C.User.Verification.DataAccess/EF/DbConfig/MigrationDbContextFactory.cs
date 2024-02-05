using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.DataAccess.EF.DbConfig
{
    public class MigrationDbContextFactory : IDesignTimeDbContextFactory<VerificationDbContext>
    {
        public VerificationDbContext CreateDbContext(string[] args = null)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VerificationDbContext>();
            var connectionString = MigrationConfig.GetConnectionString().UnSecure();
            dbContextOptionsBuilder.UseSqlServer(connectionString, options => { options.EnableRetryOnFailure(3); })
                                   .EnableSensitiveDataLogging()
                                   .ReplaceService<IMigrationsSqlGenerator, SqlGenerator>();

            var usageDbContext = new VerificationDbContext(dbContextOptionsBuilder.Options);
            return usageDbContext;
        }
    }
}
