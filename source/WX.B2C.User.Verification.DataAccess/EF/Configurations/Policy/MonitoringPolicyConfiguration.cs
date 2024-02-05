using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class MonitoringPolicyConfiguration : IEntityTypeConfiguration<MonitoringPolicy>
    {
        public void Configure(EntityTypeBuilder<MonitoringPolicy> builder)
        {
            builder.ToTable("Monitoring", Schemas.Policy);
            builder.HasKey(policy => policy.Id);

            builder.HasIndex(policy => new { policy.RegionType, policy.Region })
                   .IsUnique();

            builder.Property(policy => policy.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(policy => policy.RegionType)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(policy => policy.Region)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(SeedData.MonitoringPolicies);
        }
    }
}