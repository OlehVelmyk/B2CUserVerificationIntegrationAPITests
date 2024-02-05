using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable("Files");

            builder.HasKey(details => details.Id);
            builder.Property(details => details.UserId).IsRequired();
            builder.Property(details => details.Crc32Checksum).IsRequired(false);

            builder.Property(details => details.FileName)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(details => details.Status)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(details => details.Provider)
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);

            builder.Property(details => details.ExternalId)
                   .HasMaxLength(50);

            builder.HasIndex(file => new { file.UserId, file.Crc32Checksum });
        }
    }
}
