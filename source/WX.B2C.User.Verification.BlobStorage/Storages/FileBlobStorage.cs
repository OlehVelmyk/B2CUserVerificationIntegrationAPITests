using System;
using System.IO;
using System.Threading.Tasks;
using WX.B2C.User.Verification.BlobStorage.Extenstions;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.BlobStorage.Storages
{
    internal class FileBlobStorage : IFileBlobStorage
    {
        private const string BlobContainerName = "documents";
        private const string ExpiryTimeInMinutes = nameof(ExpiryTimeInMinutes);

        private readonly IBlobStorage _blobStorage;
        private readonly IHostSettingsProvider _hostSettingsProvider;
        
        public FileBlobStorage(IBlobStorage blobStorage, IHostSettingsProvider hostSettingsProvider)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
        }

        public Task UploadAsync(BlobFileDto fileDto, byte[] file) =>
            _blobStorage.UploadAsync(BlobContainerName, fileDto.BlobStoragePath(), file);

        public Task<Stream> DownloadAsync(BlobFileDto blobFileDto) =>
            _blobStorage.DownloadAsync(BlobContainerName, blobFileDto.BlobStoragePath());

        public Task DeleteAsync(BlobFileDto fileDto) =>
            _blobStorage.DeleteAsync(BlobContainerName, fileDto.BlobStoragePath());

        public Task<bool> ExistsAsync(Guid userId, Guid fileId, string fileName) =>
            _blobStorage.ExistsAsync(BlobContainerName, StoragePathExtensions.BlobStoragePath(userId, fileId, fileName));

        public Task<string> GenerateDownloadHrefAsync(BlobFileDto fileDto)
        {
            var expiryTimeString = _hostSettingsProvider.GetSetting(ExpiryTimeInMinutes);
            if (!int.TryParse(expiryTimeString, out var expiryTime))
                throw new ArgumentException($"Parameter {ExpiryTimeInMinutes} in host settings is invalid int.");

            return _blobStorage.GenerateSasUriAsync(BlobContainerName, fileDto.BlobStoragePath(), expiryTime);
        }
    }
}