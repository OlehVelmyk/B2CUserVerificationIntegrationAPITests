using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class CheckConfiguration : IEntityTypeConfiguration<Check>
    {
        public void Configure(EntityTypeBuilder<Check> builder)
        {
            builder.ToTable("Checks");

            builder.HasKey(check => check.Id);

            builder.Property(check => check.UserId).IsRequired();
            builder.Property(check => check.VariantId).IsRequired();

            builder.Property(check => check.Type)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(50);

            builder.Property(check => check.State)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(check => check.Result)
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(check => check.Provider)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);

            builder.Property(check => check.InputData)
                   .HasTypeNamedJsonSerializerConversion();

            builder.Property(check => check.OutputData);

            builder.Property(check => check.ExternalId)
                   .HasMaxLength(50);

            builder.Property(check => check.ExternalData)
                   .HasTypeNamedJsonSerializerConversion();

            builder.Property(check => check.Errors)
                   .HasJsonSerializerConversion();

            builder.Property(check => check.Decision)
                   .HasMaxLength(30);

            builder.HasIndex(check => check.UserId);
            builder.HasIndex(check => check.VariantId);
            builder.HasIndex(check => new { check.Provider, check.ExternalId });
        }
    }
}