using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class ApplicationStateChangelogConfiguration : IEntityTypeConfiguration<ApplicationStateChangelog>
    {
        public void Configure(EntityTypeBuilder<ApplicationStateChangelog> builder)
        {
            builder.ToTable("ApplicationStateChangelog");

            builder.HasKey(changelog => changelog.ApplicationId);

            builder.Property(changelog => changelog.ApplicationId).IsRequired();


            builder.HasOne(changelog => changelog.Application)
                   .WithMany()
                   .HasForeignKey(changelog => changelog.ApplicationId);

            builder.HasIndex(changelog => new { changelog.ApplicationId, changelog.LastApprovedDate });
        }
    }
}