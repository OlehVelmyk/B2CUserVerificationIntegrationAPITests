using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class PolicyTaskConfiguration : IEntityTypeConfiguration<PolicyTask>
    {
        public void Configure(EntityTypeBuilder<PolicyTask> builder)
        {
            builder.ToTable("PolicyTasks", Schemas.Policy);
            builder.HasKey(task => new {task.PolicyId, task.TaskVariantId});

            builder.HasOne(policyTask => policyTask.TaskVariant)
                   .WithMany()
                   .HasForeignKey(task => task.TaskVariantId);

            builder.HasData(SeedData.PolicyTasks);
        }
    }
}