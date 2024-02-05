using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class ApplicationTaskConfiguration : IEntityTypeConfiguration<ApplicationTask>
    {
        public void Configure(EntityTypeBuilder<ApplicationTask> builder)
        {
            builder.ToTable("ApplicationTasks");

            builder.HasKey(details => new { details.ApplicationId, details.TaskId });
        }
    }
}
