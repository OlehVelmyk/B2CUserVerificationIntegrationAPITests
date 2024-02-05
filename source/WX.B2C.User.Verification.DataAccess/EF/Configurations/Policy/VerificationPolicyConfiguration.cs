using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class VerificationPolicyConfiguration : IEntityTypeConfiguration<VerificationPolicy>
    {
        public void Configure(EntityTypeBuilder<VerificationPolicy> builder)
        {
            builder.ToTable("Verifications", Schemas.Policy);
            builder.HasKey(policy => policy.Id);

            builder.HasIndex(policy => new { policy.RegionType, policy.Region })
                   .IsUnique();

            builder.Property(policy => policy.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(policy => policy.RegionType)
                   .IsRequired()
                   // TODO: Investigate and fix
                   // Can not save RegionType as string because we need to order it by numbers (in VerificationPolicyStorage).
                   // May be smart enum can help to contain in db string, but sort by number.
                   //.HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.Property(policy => policy.Region)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(policy => policy.RejectionPolicy)
                   .HasJsonSerializerConversion();

            builder.HasMany(policy => policy.Tasks)
                   .WithOne()
                   .HasForeignKey(task => task.PolicyId);

            builder.HasData(SeedData.VerificationPolicies);
        }
    }
}