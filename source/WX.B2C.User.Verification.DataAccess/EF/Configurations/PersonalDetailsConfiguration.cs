using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class PersonalDetailsConfiguration : IEntityTypeConfiguration<PersonalDetails>
    {
        public void Configure(EntityTypeBuilder<PersonalDetails> builder)
        {
            builder.ToTable("PersonalDetails");
            builder.HasKey(details => details.UserId);

            builder.Property(x => x.FirstName)
                   .HasMaxLength(150);

            builder.Property(x => x.LastName)
                   .HasMaxLength(150);

            builder.Property(x => x.Email)
                   .HasMaxLength(385);

            builder.Property(x => x.Nationality)
                   .HasMaxLength(2);

            builder.HasOne(details => details.ResidenceAddress)
                   .WithOne()
                   .HasForeignKey<ResidenceAddress>(address => address.UserId);
        }
    }
}