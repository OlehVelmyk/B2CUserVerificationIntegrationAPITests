using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.EF.Configurations
{
    internal class DocumentFileConfiguration : IEntityTypeConfiguration<DocumentFile>
    {
        public void Configure(EntityTypeBuilder<DocumentFile> builder)
        {
            builder.ToTable("DocumentFiles");

            builder.HasKey(documentFile => documentFile.Id);
        }
    }
}
