using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class PolicyCheckConfiguration : IEntityTypeConfiguration<PolicyCheckVariant>
    {
        public void Configure(EntityTypeBuilder<PolicyCheckVariant> builder)
        {
            builder.ToTable("ChecksVariants", Schemas.Policy);
            builder.HasKey(check => check.Id);

            builder.Property(check => check.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(check => check.Type)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(50);

            builder.Property(check => check.Provider)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);

            builder.Property(check => check.FailResultType)
                   .HasEnumToStringConversion()
                   .HasMaxLength(30);

            builder.Property(check => check.Config);
            builder.Property(check => check.FailResult);

            builder.Property(check => check.RunPolicy)
                   .HasJsonSerializerConversion();

            builder.HasData(SeedData.ChecksVariants);
        }
    }
}
