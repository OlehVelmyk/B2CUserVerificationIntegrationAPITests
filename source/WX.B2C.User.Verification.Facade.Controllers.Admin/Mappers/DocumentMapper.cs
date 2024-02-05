using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IDocumentMapper
    {
        Dtos.DocumentDto Map(DocumentDto document);

        SubmitDocumentDto Map(SubmitDocumentRequest request);
    }

    internal class DocumentMapper : IDocumentMapper
    {
        public SubmitDocumentDto Map(SubmitDocumentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var fileIds = request.Files
                               .Select(Guid.Parse)
                               .ToArray();

            return new SubmitDocumentDto
            {
                Category = request.Category,
                Type = request.Type,
                FileIds = fileIds
            };
        }

        public Dtos.DocumentDto Map(DocumentDto document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return new Dtos.DocumentDto
            {
                Id = document.Id,
                Type = document.Type,
                Category = document.Category.ToString(),
                SubmittedAt = document.CreatedAt,
                Files = document.Files.Select(Map).ToArray()
            };
        }

        private static Dtos.DocumentFileDto Map(DocumentFileDto documentFile)
        {
            if (documentFile == null)
                throw new ArgumentNullException(nameof(documentFile));

            return new Dtos.DocumentFileDto
            {
                Id = documentFile.FileId,
                FileName = documentFile.FileName,
                CreatedAt = documentFile.CreatedAt
            };
        }
    }
}