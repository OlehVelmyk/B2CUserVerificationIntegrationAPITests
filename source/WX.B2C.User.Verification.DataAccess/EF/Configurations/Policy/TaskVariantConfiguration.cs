using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class TaskVariantConfiguration : IEntityTypeConfiguration<TaskVariant>
    {
        public void Configure(EntityTypeBuilder<TaskVariant> builder)
        {
            builder.ToTable("Tasks", Schemas.Policy);

            builder.HasKey(task => task.Id);
            builder.Property(task => task.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(task => task.Type)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(30);

            builder.Property(task => task.Priority)
                   .IsRequired();

            builder.Property(task => task.AutoCompletePolicy)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10)
                   .HasDefaultValue(AutoCompletePolicy.None);

            builder.Property(task => task.CollectionSteps)
                   .HasJsonSerializerConversion();

            builder.HasMany(task => task.ChecksVariants)
                   .WithOne()
                   .HasForeignKey(taskCheckVariant => taskCheckVariant.TaskId);
            
            builder.HasData(SeedData.Tasks);
        }
    }
}