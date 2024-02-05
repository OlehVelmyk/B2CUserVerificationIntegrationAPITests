using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes");

            builder.HasKey(note => note.Id);
            builder.Property(note => note.SubjectId).IsRequired();
            builder.Property(note => note.AuthorEmail).IsRequired();
            builder.Property(note => note.CreatedAt).IsRequired();
            builder.Property(note => note.SubjectType)
                .IsRequired()
                .HasEnumToStringConversion()
                .HasMaxLength(20);
            builder.Property(p => p.Timestamp).IsRowVersion()
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired();

            builder.Property(note => note.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasIndex(note => note.SubjectType);
            builder.HasIndex(note => note.SubjectId);
            builder.HasIndex(note => note.AuthorEmail);
        }
    }
}
