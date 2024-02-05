using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class VerificationTaskConfiguration : IEntityTypeConfiguration<VerificationTask>
    {
        public void Configure(EntityTypeBuilder<VerificationTask> builder)
        {
            builder.ToTable("VerificationTasks");

            builder.HasKey(task => task.Id);

            builder.Property(task => task.UserId).IsRequired();
            builder.Property(task => task.VariantId).IsRequired();
            builder.Property(task => task.IsExpired).IsRequired();
            builder.Property(task => task.CreationDate).IsRequired();

            builder.Property(task => task.Type)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(30);

            builder.Property(task => task.State)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(task => task.Result)
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(task => task.ExpirationReason)
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);

            builder.Property(task => task.AcceptanceCheckIds)
                   .HasJsonSerializerConversion();

            builder.HasMany(x => x.CollectionSteps)
                   .WithOne()
                   .HasForeignKey(x => x.TaskId);

            builder.HasMany(x => x.PerformedChecks)
                   .WithOne()
                   .HasForeignKey(d => d.TaskId);

            builder.HasIndex(task => new { task.UserId, task.VariantId });
            builder.HasIndex(task => new { task.UserId, task.Type });
        }
    }
}