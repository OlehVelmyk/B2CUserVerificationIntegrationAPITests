using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;

namespace WX.B2C.User.Verification.DataAccess.EF
{
    public class VerificationDbContext : DbContext
    {
        public VerificationDbContext() { }

        public VerificationDbContext(DbContextOptions contextOptions)
            : base(contextOptions) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureDateTimeKind();

            var assembly = Assembly.GetAssembly(typeof(VerificationDbContext));
            builder.ApplyConfigurationsFromAssembly(assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetAuditables();
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch (DbUpdateConcurrencyException exc)
            {
                throw new DatabaseConcurrencyException(exc);
            }
            catch (DbUpdateException exc)
            {
                throw new DatabaseException(exc);
            }
        }
    }
}