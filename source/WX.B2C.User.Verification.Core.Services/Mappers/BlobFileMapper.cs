using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Services.Mappers
{
    internal class BlobFileMapper : IBlobFileMapper
    {
        public BlobFileDto Map(Guid userId, DocumentFileDto documentFileDto)
        {
            if (documentFileDto == null)
                throw new ArgumentNullException(nameof(documentFileDto));

            return new BlobFileDto
            {
                FileId = documentFileDto.FileId,
                FileName = documentFileDto.FileName,
                UserId = userId
            };
        }

        public BlobFileDto Map(FileDto fileDto)
        {
            if (fileDto == null)
                throw new ArgumentNullException(nameof(fileDto));

            return new BlobFileDto
            {
                FileId = fileDto.Id,
                FileName = fileDto.FileName,
                UserId = fileDto.UserId
            };
        }
    }
}