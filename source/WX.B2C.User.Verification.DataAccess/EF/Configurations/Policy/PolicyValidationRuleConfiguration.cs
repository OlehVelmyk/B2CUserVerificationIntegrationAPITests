using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class PolicyValidationRuleConfiguration : IEntityTypeConfiguration<PolicyValidationRule>
    {
        public void Configure(EntityTypeBuilder<PolicyValidationRule> builder)
        {
            builder.ToTable("PolicyValidationRules", Schemas.Policy);
            builder.HasKey(rule => new { rule.ValidationPolicyId, rule.ValidationRuleId });

            builder.HasOne(rule => rule.Rule)
                   .WithMany()
                   .HasForeignKey(rule => rule.ValidationRuleId);

            builder.HasData(Seed.SeedData.PolicyValidationRules);
        }
    }
}