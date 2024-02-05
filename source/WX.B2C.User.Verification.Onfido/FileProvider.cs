using System;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Mappers;

namespace WX.B2C.User.Verification.Onfido
{
    internal class FileProvider : BaseOnfidoGateway, IExternalFileProvider
    {
        private readonly IOnfidoApiClientFactory _onfidoApiClientFactory;
        private readonly IOnfidoDocumentMapper _documentMapper;

        public FileProvider(
            IOnfidoApiClientFactory onfidoApiClientFactory,
            IOnfidoDocumentMapper documentMapper,
            ILogger logger) : base(logger)
        {
            _onfidoApiClientFactory = onfidoApiClientFactory ?? throw new ArgumentNullException(nameof(onfidoApiClientFactory));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
        }

        public Task<DownloadedFile> DownloadAsync(string profileId, ExternalFileDto externalFile)
        {
            if (profileId == null)
                throw new ArgumentNullException(nameof(profileId));
            if (externalFile == null)
                throw new ArgumentNullException(nameof(externalFile));

            var externalFileId = externalFile.Id;

            return externalFile.DocumentType switch
            {
                nameof(SelfieDocumentType.Photo) => DownloadSelfieAsync(profileId, externalFileId, SelfieType.Photo),
                nameof(SelfieDocumentType.Video) => DownloadSelfieAsync(profileId, externalFileId, SelfieType.Video),
                _ => DownloadDocumentAsync(profileId, externalFileId)
            };
        }

        public Task<string> UploadAsync(string profileId, UploadExternalFileDto uploadFile)
        {
            var file = new FileToUpload(uploadFile.Stream, uploadFile.Name, uploadFile.ContentType);

            return uploadFile.DocumentType switch
            {
                nameof(SelfieDocumentType.Photo) => UploadPhotoAsync(profileId, file),
                nameof(SelfieDocumentType.Video) => throw new NotSupportedException("Cannot uplad video to Onfido."),
                _ => UploadDocumentAsync(profileId, uploadFile.DocumentType, file)
            };
        }

        private async Task<DownloadedFile> DownloadDocumentAsync(string applicantId, string externalFileId)
        {
            using var client = _onfidoApiClientFactory.Create();
            var document = await HandleAsync(
                requestFactory: () => (applicantId, externalFileId),
                requestInvoker: client.DownloadDocumentAsync);

            if (document == null)
                throw new ExternalFileNotFoundException(externalFileId, applicantId, ExternalFileProviderType.Onfido);

            return document;
        }

        private async Task<DownloadedFile> DownloadSelfieAsync(string applicantId, string externalFileId, SelfieType selfieType)
        {
            var file = selfieType switch
            {
                SelfieType.Photo => await FindLivePhotosAsync(applicantId, externalFileId),
                SelfieType.Video => await FindLiveVideoAsync(applicantId, externalFileId),
                _ => throw new ArgumentOutOfRangeException(nameof(selfieType), selfieType, "Unsupported selfie type.")
            };

            if (file == null)
                throw new ExternalFileNotFoundException(externalFileId, applicantId, ExternalFileProviderType.Onfido);

            return file;
        }

        private async Task<string> UploadPhotoAsync(string applicantId, FileToUpload file)
        {
            using var client = _onfidoApiClientFactory.Create();
            return await HandleAsync(
                requestFactory: () => (applicantId, file),
                requestInvoker: client.LivePhotos.UploadAsync,
                responseMapper: photo => photo.Id);
        }

        private async Task<string> UploadDocumentAsync(string applicantId, string documentType, FileToUpload file)
        {
            using var client = _onfidoApiClientFactory.Create();
            var onfidoType = _documentMapper.Map(documentType);

            return await HandleAsync(
                requestFactory: () => (applicantId, onfidoType, file),
                requestInvoker: client.Documents.UploadAsync,
                responseMapper: photo => photo.Id);
        }

        private async Task<DownloadedFile> FindLivePhotosAsync(string applicantId, string externalFileId)
        {
            using var client = _onfidoApiClientFactory.Create();
            return await HandleAsync(
                requestFactory: () => (applicantId, externalFileId),
                requestInvoker: client.FindLivePhotosAsync);
        }

        private async Task<DownloadedFile> FindLiveVideoAsync(string applicantId, string externalFileId)
        {
            using var client = _onfidoApiClientFactory.Create();
            return await HandleAsync(
                requestFactory: () => (applicantId, externalFileId),
                requestInvoker: client.FindLiveVideoAsync);
        }
    }
}