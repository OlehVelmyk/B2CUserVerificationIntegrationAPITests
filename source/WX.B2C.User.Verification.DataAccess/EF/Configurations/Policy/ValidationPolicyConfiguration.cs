using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class ValidationPolicyConfiguration : IEntityTypeConfiguration<ValidationPolicy>
    {
        public void Configure(EntityTypeBuilder<ValidationPolicy> builder)
        {
            builder.ToTable("ValidationPolicies", Schemas.Policy);
            builder.HasKey(policy => policy.Id);
            
            builder.Property(policy => policy.Region)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(policy => policy.RegionType)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.HasData(Seed.SeedData.ValidationPolicies);
        }
    }
}