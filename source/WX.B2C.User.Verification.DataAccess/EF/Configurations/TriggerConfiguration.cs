using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class TriggerConfiguration : IEntityTypeConfiguration<Trigger>
    {
        public void Configure(EntityTypeBuilder<Trigger> builder)
        {
            builder.ToTable("Triggers");

            builder.HasKey(trigger => trigger.Id);

            builder.Property(trigger => trigger.ApplicationId)
                   .IsRequired();

            builder.Property(trigger => trigger.State)
                   .IsRequired()
                   .HasEnumToStringConversion();

            builder.Property(trigger => trigger.UserId)
                   .IsRequired();

            builder.Property(trigger => trigger.VariantId)
                   .IsRequired();

            builder.Property(trigger => trigger.Context)
                   .HasTypeNamedJsonSerializerConversion();

            builder.HasIndex(trigger => new { trigger.VariantId, trigger.ApplicationId, trigger.State }).IsUnique(false);
            builder.HasIndex(trigger => trigger.ApplicationId).IsUnique(false);
        }
    }
}