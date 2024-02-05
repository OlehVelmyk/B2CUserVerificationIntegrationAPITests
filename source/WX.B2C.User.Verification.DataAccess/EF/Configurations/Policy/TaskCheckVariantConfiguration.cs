using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class TaskCheckVariantConfiguration : IEntityTypeConfiguration<TaskCheckVariant>
    {
        public void Configure(EntityTypeBuilder<TaskCheckVariant> builder)
        {
            builder.ToTable("TaskCheckVariants", Schemas.Policy);
            builder.HasKey(taskCheckVariant => new { taskCheckVariant.TaskId, taskCheckVariant.CheckVariantId });

            builder.HasData(Seed.SeedData.TaskCheckVariants);
        }
    }
}