using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Mappers
{
    public interface IFileMapper
    {
        UploadedFileDto Map(IFormFile file);

        ExternalFileDto Map(string fileId, ExternalFileProviderType provider, string type);
    }

    internal class FileMapper : IFileMapper
    {
        public UploadedFileDto Map(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            using var ms = new MemoryStream();
            file.CopyTo(ms);

            return new UploadedFileDto
            {
                Name = file.FileName,
                File = ms.ToArray()
            };
        }

        public ExternalFileDto Map(string fileId, ExternalFileProviderType provider, string type)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentNullException(nameof(fileId));

            return new ExternalFileDto { Id = fileId, Provider = provider, DocumentType = type};
        }
    }
}