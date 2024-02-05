using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IFileMapper
    {
        UploadedFileDto Map(IFormFile file);

        UploadExternalFileDto Map(UploadDocumentFileRequest request);
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

        public UploadExternalFileDto Map(UploadDocumentFileRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new UploadExternalFileDto
            {
                Name = request.File.Name,
                ContentType = request.File.ContentType,
                Stream = request.File.OpenReadStream(),
                DocumentType = request.DocumentType,
                Provider = request.Provider.GetValueOrDefault()
            };
        }
    }
}