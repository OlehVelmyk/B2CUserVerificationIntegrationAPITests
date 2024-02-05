using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Seed;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations.Policy
{
    internal class TriggerVariantConfiguration : IEntityTypeConfiguration<TriggerVariant>
    {
        public void Configure(EntityTypeBuilder<TriggerVariant> builder)
        {
            builder.ToTable("Triggers", Schemas.Policy);
            builder.HasKey(trigger => trigger.Id);

            builder.Property(trigger => trigger.Name)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(trigger => trigger.Schedule)
                   .HasJsonSerializerConversion();

            builder.Property(trigger => trigger.Preconditions)
                   .HasJsonSerializerConversion();            
            
            builder.Property(trigger => trigger.Conditions)
                   .HasJsonSerializerConversion();

            builder.Property(trigger => trigger.Commands)
                   .HasJsonSerializerConversion();

            builder.HasIndex(trigger => trigger.PolicyId)
                   .IsUnique(false);

            builder.HasData(SeedData.Triggers);
        }
    }
}