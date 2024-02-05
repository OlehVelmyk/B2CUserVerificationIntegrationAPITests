using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class TaskCollectionStepConfiguration : IEntityTypeConfiguration<TaskCollectionStep>
    {
        public void Configure(EntityTypeBuilder<TaskCollectionStep> builder)
        {
            builder.ToTable("TaskCollectionSteps");

            builder.HasKey(details => new { details.TaskId, details.StepId });
        }
    }
}
