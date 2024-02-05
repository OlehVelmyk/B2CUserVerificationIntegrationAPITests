using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class ValidationRuleConfiguration : IEntityTypeConfiguration<ValidationRule>
    {
        public void Configure(EntityTypeBuilder<ValidationRule> builder)
        {
            builder.ToTable("ValidationRules", Schemas.Policy);
            builder.HasKey(rule => rule.Id);

            builder.Property(rule => rule.RuleSubject)
                   .HasMaxLength(50);

            builder.Property(rule => rule.RuleType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(rule => rule.Validation).IsRequired();

            builder.HasData(Seed.SeedData.ValidationRules);
        }
    }
}