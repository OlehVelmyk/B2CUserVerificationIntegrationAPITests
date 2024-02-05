using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder.ToTable("Reminders");

            builder.HasKey(step => step.Id);

            builder.Property(step => step.UserId).IsRequired();
            builder.Property(step => step.TargetId).IsRequired();
            builder.Property(step => step.SentAt).IsRequired();

            builder.HasIndex(step => new { step.UserId, step.TargetId });
        }
    }
}
