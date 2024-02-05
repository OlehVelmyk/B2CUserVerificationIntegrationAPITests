using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class AddressConfiguration : IEntityTypeConfiguration<ResidenceAddress>
    {
        public void Configure(EntityTypeBuilder<ResidenceAddress> builder)
        {
            builder.ToTable("ResidenceAddresses");

            builder.HasKey(details => details.UserId);

            builder.Property(x => x.Country)
                   .IsRequired()
                   .HasMaxLength(2);

            builder.Property(x => x.State)
                   .HasMaxLength(100);

            builder.Property(x => x.City)
                   .HasMaxLength(100);

            builder.Property(x => x.Line1)
                   .HasMaxLength(250);

            builder.Property(x => x.Line2)
                   .HasMaxLength(250);

            builder.Property(x => x.ZipCode)
                   .HasMaxLength(20);

            builder.HasIndex(x => x.Country)
                   .HasAnnotation("SqlServer:Online", true);
        }
    }
}