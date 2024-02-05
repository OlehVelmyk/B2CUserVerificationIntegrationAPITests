using System;
using Optional;
using Optional.Unsafe;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IFileMapper
    {
        File Map(FileDto fileDto);

        FileDto Map(File entity);

        void Update(FileDto fileDto, File entity);
    }

    internal class FileMapper : IFileMapper
    {
        public File Map(FileDto fileDto)
        {
            if (fileDto == null)
                throw new ArgumentNullException(nameof(fileDto));

            var file = new File { Id = fileDto.Id };
            Update(fileDto, file);
            return file;
        }

        public FileDto Map(File entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new FileDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                FileName = entity.FileName,
                Status = entity.Status,
                Provider = entity.Provider,
                ExternalId = entity.ExternalId,
                Crc32Checksum = entity.Crc32Checksum.ToOption()
            };
        }

        public void Update(FileDto fileDto, File entity)
        {
            if (fileDto == null)
                throw new ArgumentNullException(nameof(fileDto));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.UserId = fileDto.UserId;
            entity.FileName = fileDto.FileName;
            entity.Status = fileDto.Status;
            entity.Provider = fileDto.Provider;
            entity.ExternalId = fileDto.ExternalId;
            entity.Crc32Checksum = fileDto.Crc32Checksum.ToNullable();
        }
    }
}
