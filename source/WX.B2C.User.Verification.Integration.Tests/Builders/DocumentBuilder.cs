using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    class DocumentBuilder
    {
        private DocumentDto _result;

        public DocumentBuilder From(DocumentSpecimen specimen)
        {
            _result = new DocumentDto
            {
                Id = specimen.Id,
                UserId = specimen.UserId,
                Status = specimen.Status,
                Category = specimen.Category,
                Type = specimen.Type,
                Files = specimen.Files.Select(documentFile => new DocumentFileDto
                {
                    Id = documentFile.Id,
                    DocumentId = specimen.Id,
                    FileId = documentFile.File.Id,
                    FileName = documentFile.File.FileName,
                    ExternalId = documentFile.File.ExternalId
                }).ToArray()
            };

            return this;
        }

        public DocumentDto Build() =>
            _result;
    }
}
