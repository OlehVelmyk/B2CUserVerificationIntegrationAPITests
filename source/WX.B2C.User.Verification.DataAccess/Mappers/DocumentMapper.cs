using System;
using Microsoft.EntityFrameworkCore.Internal;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IDocumentMapper
    {
        Document Map(DocumentDto documentDto);

        DocumentDto Map(Document entity);

        void Update(DocumentDto documentDto, Document entity);
    }

    internal class DocumentMapper : IDocumentMapper
    {
        public Document Map(DocumentDto documentDto)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto));

            var document = new Document { Id = documentDto.Id };
            Update(documentDto, document);
            return document;
        }

        public DocumentDto Map(Document entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var files = entity.Files?.MapArray(Map) ?? Array.Empty<DocumentFileDto>();

            return new DocumentDto
            {
                UserId = entity.UserId,
                Id = entity.Id,
                Category = entity.Category,
                Type = entity.Type,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                Files = files
            };
        }

        public void Update(DocumentDto documentDto, Document entity)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.UserId = documentDto.UserId;
            entity.Id = documentDto.Id;
            entity.Category = documentDto.Category;
            entity.Type = documentDto.Type;
            entity.Status = documentDto.Status;
            entity.Files = documentDto.Files.MapHashSet(Map);
        }

        private static DocumentFile Map(DocumentFileDto documentFileDto)
        {
            if (documentFileDto == null)
                throw new ArgumentNullException(nameof(documentFileDto));

            return new DocumentFile
            {
                Id = documentFileDto.Id,
                DocumentId = documentFileDto.DocumentId,
                FileId = documentFileDto.FileId
            };
        }

        private static DocumentFileDto Map(DocumentFile documentFile)
        {
            if (documentFile == null)
                throw new ArgumentNullException(nameof(documentFile));

            return new DocumentFileDto
            {
                Id = documentFile.Id,
                DocumentId = documentFile.DocumentId,
                DocumentType = documentFile.Document.Type,
                FileId = documentFile.File.Id,
                FileName = documentFile.File.FileName,
                ExternalId = documentFile.File.ExternalId,
                Provider = documentFile.File.Provider,
                CreatedAt = documentFile.File.CreatedAt,
            };
        }
    }
}