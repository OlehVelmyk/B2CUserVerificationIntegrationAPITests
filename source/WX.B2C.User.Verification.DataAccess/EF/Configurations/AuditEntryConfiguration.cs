using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Audit;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> builder)
        {
            builder.ToTable("AuditEntries");

            builder.HasKey(audit => audit.Id);

            builder.Property(audit => audit.UserId).IsRequired();
            builder.Property(audit => audit.EntryKey).IsRequired();
            builder.Property(audit => audit.CreatedAt).IsRequired();
            builder.Property(audit => audit.Data).IsRequired();

            builder.Property(audit => audit.EntryType)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(40);

            builder.Property(audit => audit.EventType)
                   .IsRequired()
                   .HasMaxLength(40);

            builder.Property(audit => audit.Initiator)
                   .IsRequired()
                   .HasMaxLength(385);

            builder.Property(audit => audit.Reason)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.HasIndex(audit => audit.UserId);
            builder.HasIndex(audit => new { audit.UserId, audit.EntryType });
            builder.HasIndex(audit => audit.CreatedAt);
        }
    }
}