using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class CollectionStepConfiguration : IEntityTypeConfiguration<CollectionStep>
    {
        public void Configure(EntityTypeBuilder<CollectionStep> builder)
        {
            builder.ToTable("CollectionSteps");

            builder.HasKey(step => step.Id);

            builder.Property(step => step.UserId).IsRequired();
            builder.Property(step => step.IsRequired).IsRequired();
            builder.Property(step => step.IsReviewNeeded).IsRequired();

            builder.Property(step => step.State)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);
            
            builder.Property(step => step.ReviewResult)
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(step => step.XPath)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(step => new { step.UserId, step.XPath }).IsUnique(false);
        }
    }
}