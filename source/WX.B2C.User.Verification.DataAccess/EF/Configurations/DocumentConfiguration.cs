using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(document => document.Id);
            builder.Property(document => document.UserId).IsRequired();

            builder.Property(document => document.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(document => document.Category)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(20);

            builder.Property(document => document.Status)
                   .IsRequired()
                   .HasEnumToStringConversion()
                   .HasMaxLength(10);

            builder.HasMany(document => document.Files)
                   .WithOne(file => file.Document)
                   .HasForeignKey(file => file.DocumentId);

            builder.HasIndex(document => new { document.UserId, document.Category });
        }
    }
}