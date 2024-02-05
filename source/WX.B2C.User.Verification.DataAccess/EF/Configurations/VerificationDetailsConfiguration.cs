using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class VerificationDetailsConfiguration : IEntityTypeConfiguration<VerificationDetails>
    {
        public void Configure(EntityTypeBuilder<VerificationDetails> builder)
        {
            builder.ToTable("VerificationDetails");

            builder.HasKey(details => details.UserId);

            builder.Property(details => details.IpAddress)
                   .HasMaxLength(40);

            builder.Property(details => details.TaxResidence)
                   .HasJsonSerializerConversion();

            builder.Property(details => details.Tin)
                   .HasJsonSerializerConversion()
                   .HasMaxLength(150);

            builder.Property(details => details.RiskLevel)
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(details => details.IdDocumentNumber)
                   .HasMaxLength(50);

            builder.Property(details => details.IdDocumentNumberType)
                   .HasMaxLength(50);

            builder.Property(details => details.Nationality)
                   .HasMaxLength(2);

            builder.Property(details => details.PoiIssuingCountry)
                   .HasMaxLength(2);

            builder.Property(details => details.ResolvedCountryCode)
                   .HasMaxLength(2);

            builder.Property(details => details.PlaceOfBirth)
                   .HasMaxLength(2);
        }
    }
}