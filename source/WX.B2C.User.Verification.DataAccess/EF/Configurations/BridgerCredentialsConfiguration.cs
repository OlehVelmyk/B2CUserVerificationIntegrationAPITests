using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class BridgerCredentialsConfiguration : IEntityTypeConfiguration<BridgerCredentials>
    {
        public void Configure(EntityTypeBuilder<BridgerCredentials> builder)
        {
            builder.ToTable("BridgerCredentials");

            builder.HasKey(details => details.Id);

            builder.Property(details => details.UserId)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(details => details.EncryptedPassword)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
