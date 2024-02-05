using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface IDocumentMapper
    {
        Dtos.DocumentDto Map(DocumentDto document, (Guid Key, string Value)[] hrefs);
    }

    internal class DocumentMapper : IDocumentMapper
    {
        public Dtos.DocumentDto Map(DocumentDto document, (Guid Key, string Value)[] hrefs)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            
            var files = document.Files.Join(hrefs,
                                            file => file.FileId,
                                            href => href.Key,
                                            (file, href) => Map(file, href.Value));
            return new Dtos.DocumentDto
            {
                Id = document.Id,
                Type = document.Type,
                Category = document.Category.ToString(),
                SubmittedAt = document.CreatedAt,
                Files = files.ToArray()
            };
        }

        private static Dtos.DocumentFileDto Map(DocumentFileDto documentFile, string downloadHref)
        {
            if (documentFile == null)
                throw new ArgumentNullException(nameof(documentFile));

            return new Dtos.DocumentFileDto
            {
                FileName = documentFile.FileName,
                CreatedAt = documentFile.CreatedAt,
                DownloadHref = downloadHref
            };
        }
    }
}