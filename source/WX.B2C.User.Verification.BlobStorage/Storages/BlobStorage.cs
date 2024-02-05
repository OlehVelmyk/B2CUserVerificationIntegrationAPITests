using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Serilog;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.BlobStorage.Storages
{
    internal interface IBlobStorage
    {
        Task UploadAsync(string blobContainerName, string path, byte[] file);

        Task<Stream> DownloadAsync(string blobContainerName, string path);

        Task DeleteAsync(string blobContainerName, string path);

        Task<bool> ExistsAsync(string blobContainerName, string path);

        Task<string> GenerateSasUriAsync(string blobContainerName, string path, int expiryTimeInMinutes);

        Task AppendAsync(string blobContainerName, string path, Stream fileBlock);
    }

    internal class BlobStorage : IBlobStorage
    {
        private readonly IAppConfig _appConfig;
        private readonly IBlobContainerClientFactory _blobContainerClientFactory;
        private readonly ILogger _logger;

        public BlobStorage(IAppConfig appConfig, IBlobContainerClientFactory blobContainerClientFactory, ILogger logger)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _blobContainerClientFactory = blobContainerClientFactory ?? throw new ArgumentNullException(nameof(blobContainerClientFactory));
            _logger = logger?.ForContext<BlobStorage>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UploadAsync(string blobContainerName, string path, byte[] file)
        {
            if (blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            
            await blob.UploadAsync(new BinaryData(file));

            _logger.Information("Uploaded file {Container}/{Path} ", blobContainerName, path);
        }
        
        public async Task AppendAsync(string blobContainerName, string path, Stream fileBlock)
        {
            if (blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (fileBlock == null)
                throw new ArgumentNullException(nameof(fileBlock));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetAppendBlobClient(path);
            await blob.CreateIfNotExistsAsync();
            await blob.AppendBlockAsync(fileBlock);
        }

        public async Task<Stream> DownloadAsync(string blobContainerName, string path)
        {
            if(blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            if (!await blob.ExistsAsync())
                throw new BlobStorageFileNotFoundException(blobContainerName, path);

            var result = await blob.DownloadAsync();
            return result.Value.Content;
        }

        public async Task DeleteAsync(string blobContainerName, string path)
        {
            if (blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            var isDeleted = await blob.DeleteIfExistsAsync();
            
            if (isDeleted)
                _logger.Information("Deleted file {Container}/{Path} ", blobContainerName, path);
        }

        public async Task<bool> ExistsAsync(string blobContainerName, string path)
        {
            if (blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            return await blob.ExistsAsync();
        }

        public async Task<string> GenerateSasUriAsync(string blobContainerName, string path, int expiryTimeInMinutes)
        {
            if (blobContainerName == null)
                throw new ArgumentNullException(nameof(blobContainerName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (expiryTimeInMinutes <= 0)
                throw new ArgumentException("Should be positive.", nameof(expiryTimeInMinutes));

            var container = await _blobContainerClientFactory.CreateAsync(_appConfig.StorageConnectionString.UnSecure(), blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            if (!blob.CanGenerateSasUri)
                throw new InvalidOperationException("Cannot generate SAS uri.");

            var startsOn = DateTimeOffset.UtcNow;
            var expiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryTimeInMinutes);
            var sasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, expiresOn)
            {
                StartsOn = startsOn
            };

            var sasUri = blob.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
    }
}