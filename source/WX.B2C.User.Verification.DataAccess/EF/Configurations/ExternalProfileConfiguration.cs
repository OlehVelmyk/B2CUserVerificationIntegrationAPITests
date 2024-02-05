using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class ExternalProfileConfiguration : IEntityTypeConfiguration<ExternalProfile>
    {
        public void Configure(EntityTypeBuilder<ExternalProfile> builder)
        {
            builder.ToTable("ExternalProfiles");
            builder.HasKey(info => new { info.UserId, info.Provider });

            builder.Property(info => info.UserId).IsRequired();

            builder.Property(info => info.ExternalId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(info => info.Provider)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);
        }
    }
}