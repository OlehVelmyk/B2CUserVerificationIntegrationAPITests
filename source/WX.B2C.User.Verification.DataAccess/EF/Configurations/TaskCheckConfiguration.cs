using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class TaskCheckConfiguration : IEntityTypeConfiguration<TaskCheck>
    {
        public void Configure(EntityTypeBuilder<TaskCheck> builder)
        {
            builder.ToTable("TaskChecks");

            builder.HasKey(details => new { details.TaskId, details.CheckId });
        }
    }
}
