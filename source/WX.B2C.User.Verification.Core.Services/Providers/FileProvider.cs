using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services.Providers
{
    internal class FileProvider : IFileProvider
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileBlobStorage _fileBlobStorage;
        private readonly IExternalProfileProvider _externalProfileProvider;
        private readonly IIndex<ExternalFileProviderType, IExternalFileProvider> _externalFileProviders; 
        private readonly IBlobFileMapper _mapper;

        public FileProvider(IFileStorage documentStorage,
                            IFileBlobStorage fileBlobStorage,
                            IExternalProfileProvider externalProfileProvider,
                            IIndex<ExternalFileProviderType, IExternalFileProvider> externalFileProviders,
                            IBlobFileMapper mapper)
        {
            _fileBlobStorage = fileBlobStorage ?? throw new ArgumentNullException(nameof(fileBlobStorage));
            _externalFileProviders = externalFileProviders ?? throw new ArgumentNullException(nameof(externalFileProviders));
            _fileStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _externalProfileProvider = externalProfileProvider;
        }

        public async Task<Stream> DownloadAsync(FileDto fileDto)
        {
            if (fileDto == null)
                throw new ArgumentNullException(nameof(fileDto));

            var blobFileDto = _mapper.Map(fileDto);
            return await DownloadAsync(blobFileDto, fileDto.ExternalId, fileDto.Provider);
        }

        public async Task<Stream> DownloadAsync(Guid userId, DocumentFileDto documentFileDto)
        {
            if (documentFileDto == null)
                throw new ArgumentNullException(nameof(documentFileDto));

            var blobFileDto = _mapper.Map(userId, documentFileDto);
            return await DownloadAsync(blobFileDto, documentFileDto.ExternalId, documentFileDto.Provider);
        }

        public async Task<string> GenerateDownloadHrefAsync(Guid userId, DocumentFileDto documentFileDto)
        {
            if (documentFileDto == null)
                throw new ArgumentNullException(nameof(documentFileDto));

            var blobFileDto = _mapper.Map(userId, documentFileDto);
            var exists = await _fileBlobStorage.ExistsAsync(userId, documentFileDto.FileId, documentFileDto.FileName);
            if (exists)
                return await _fileBlobStorage.GenerateDownloadHrefAsync(blobFileDto);

            if (documentFileDto.Provider != ExternalFileProviderType.Onfido)
                throw new BlobStorageFileNotFoundException(documentFileDto.FileId);

            await OnfidoFallbackDownloadAsync(blobFileDto, documentFileDto.ExternalId, documentFileDto.DocumentType);
            return await _fileBlobStorage.GenerateDownloadHrefAsync(blobFileDto);
        }

        public async Task<DownloadedFile> DownloadAsync(Guid userId, ExternalFileDto externalFile)
        {
            var fileProviderType = externalFile.Provider;

            var profileProvider = MapExternalProvider(fileProviderType);
            var profile = await _externalProfileProvider.GetOrCreateAsync(userId, profileProvider);

            var fileProvider = GetFileProvider(fileProviderType);
            return await fileProvider.DownloadAsync(profile.Id, externalFile);
        }

        public async Task<string> UploadAsync(Guid userId, UploadExternalFileDto uploadFile)
        {
            var fileProviderType = uploadFile.Provider;

            var profileProvider = MapExternalProvider(fileProviderType);
            var profile = await _externalProfileProvider.GetOrCreateAsync(userId, profileProvider);

            var fileProvider = GetFileProvider(fileProviderType);
            return await fileProvider.UploadAsync(profile.Id, uploadFile);
        }

        private async Task<Stream> DownloadAsync(BlobFileDto blobFileDto, string externalId, ExternalFileProviderType? provider)
        {
            var exists = await _fileBlobStorage.ExistsAsync(blobFileDto.UserId, blobFileDto.FileId, blobFileDto.FileName);
            if (exists)
                return await _fileBlobStorage.DownloadAsync(blobFileDto);

            if (provider == ExternalFileProviderType.Onfido)
                return await OnfidoFallbackDownloadAsync(blobFileDto, externalId);

            throw new BlobStorageFileNotFoundException(blobFileDto.FileId);
        }

        private async Task<Stream> OnfidoFallbackDownloadAsync(BlobFileDto fileDto, string externalId)
        {
            var documentType = await _fileStorage.GetDocumentTypeAsync(fileDto.FileId);
            return await OnfidoFallbackDownloadAsync(fileDto, externalId, documentType);
        }

        private async Task<Stream> OnfidoFallbackDownloadAsync(BlobFileDto fileDto, string externalId, string documentType)
        {
            var externalFileDto = new ExternalFileDto
            {
                Id = externalId,
                Provider = ExternalFileProviderType.Onfido,
                DocumentType = documentType
            };
            var documentData = await DownloadAsync(fileDto.UserId, externalFileDto);

            await _fileBlobStorage.UploadAsync(fileDto, documentData.Data);
            return new MemoryStream(documentData.Data);
        }

        private IExternalFileProvider GetFileProvider(ExternalFileProviderType fileProviderType)
        {
            var isKnownProvider = _externalFileProviders.TryGetValue(fileProviderType, out var fileProvider);
            if (!isKnownProvider)
                throw new InvalidOperationException($"Cannot find document provider for type {fileProviderType}");

            return fileProvider;
        }

        private static ExternalProviderType MapExternalProvider(ExternalFileProviderType providerType)
        {
            return providerType switch
            {
                ExternalFileProviderType.Onfido => ExternalProviderType.Onfido,
                _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, $"Cannot match {providerType} to external provider.")
            };
        }
    }
}