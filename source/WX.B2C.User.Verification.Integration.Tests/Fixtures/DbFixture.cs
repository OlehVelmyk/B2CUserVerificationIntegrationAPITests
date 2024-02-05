using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.DataAccess.EF;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class DbFixture
    {
        public DbFixture(string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VerificationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString, options => { options.EnableRetryOnFailure(3); })
                                   .EnableSensitiveDataLogging();

            DbContext = new VerificationDbContext(dbContextOptionsBuilder.Options);
        }

        public VerificationDbContext DbContext { get; }

        public async Task Clear(params Guid[] users)
        {
            if (users.IsNullOrEmpty())
                return;

            const string sqlTemplate = @"delete from Applications where UserId in ({0});
                                         delete from VerificationTasks where UserId in ({0});
                                         delete from CollectionSteps where UserId in ({0});
                                         delete from Checks where UserId in ({0});
                                         delete from Documents where UserId in ({0});
                                         delete from Files where UserId in ({0});
                                         delete from PersonalDetails where UserId in ({0});
                                         delete from VerificationDetails where UserId in ({0});";

            var usersToDelete = string.Join(", ", users.Select(id => $"'{id}'"));
            var sql = string.Format(sqlTemplate, usersToDelete);
            await DbContext.Database.ExecuteSqlRawAsync(sql);
        }

    }
}