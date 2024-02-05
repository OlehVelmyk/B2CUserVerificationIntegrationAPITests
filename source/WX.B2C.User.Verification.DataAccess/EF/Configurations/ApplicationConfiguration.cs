using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Applications");

            builder.HasKey(details => details.Id);

            builder.Property(application => application.PolicyId).IsRequired();

            builder.Property(application => application.ProductType)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(50);

            builder.Property(application => application.State)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(application => application.PreviousState)
                   .IsRequired(false)
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(application => application.DecisionReasons)
                   .IsRequired(false)
                   .HasJsonSerializerConversion();

            builder.HasMany(x => x.RequiredTasks)
                   .WithOne()
                   .HasForeignKey(x => x.ApplicationId);

            builder.Property(application => application.IsAutomating)
                   .IsRequired();

            builder.HasIndex(details => new { details.UserId, details.ProductType })
                   .IsUnique(false);
        }
    }
}