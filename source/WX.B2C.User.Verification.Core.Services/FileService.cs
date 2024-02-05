using System;
using System.Threading.Tasks;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.Utils.Helpers;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class FileService : IFileService
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;
        private readonly IFileBlobStorage _fileBlobStorage;
        private readonly IBlobFileMapper _fileMapper;

        public FileService(
            IFileRepository fileRepository,
            IFileStorage fileStorage,
            IFileBlobStorage fileBlobStorage, 
            IBlobFileMapper fileMapper)
        {
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _fileBlobStorage = fileBlobStorage ?? throw new ArgumentNullException(nameof(fileBlobStorage));
            _fileMapper = fileMapper ?? throw new ArgumentNullException(nameof(fileMapper));
        }

        public async Task<Guid> UploadAsync(Guid userId, UploadedFileDto uploadedFileDto)
        {
            var checksum = CryptoHelper.Hash.Crc32Checksum(uploadedFileDto.File);
            var fileDto = await _fileStorage.FindAsync(userId, checksum);

            if (fileDto != null)
                return fileDto.Id;

            fileDto = new FileDto
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = uploadedFileDto.Name,
                Status = FileStatus.Uploaded,
                Crc32Checksum = checksum.Some(),
                ExternalId = uploadedFileDto.ExternalData?.Id,
                Provider = uploadedFileDto.ExternalData?.Provider
            };

            await _fileBlobStorage.UploadAsync(_fileMapper.Map(fileDto), uploadedFileDto.File);

            //TODO if saving failed - then we need to retry. Probably we need to remove from storage
            await _fileRepository.SaveAsync(fileDto);

            return fileDto.Id;
        }

        public async Task UpdateAsync(Guid userId, Guid fileId, ExternalFileData externalData)
        {
            var file = await _fileRepository.GetAsync(fileId);
            file.ExternalId = externalData.Id;
            file.Provider = externalData.Provider;
            await _fileRepository.SaveAsync(file);
        }

        public async Task SubmitAsync(Guid userId, Guid fileId)
        {
            var file = await _fileRepository.GetAsync(fileId);
            if (file.Status is FileStatus.Submitted)
                return;

            file.Status = FileStatus.Submitted;
            await _fileRepository.SaveAsync(file);
        }
    }
}