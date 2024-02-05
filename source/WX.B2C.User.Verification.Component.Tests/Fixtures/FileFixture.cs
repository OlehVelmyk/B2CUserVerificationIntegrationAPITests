using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Mountebank;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Onfido.Client;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using FileToUpload = WX.B2C.User.Verification.Component.Tests.Models.FileToUpload;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class FileFixture
    {
        private readonly PublicApiClientFactory _publicApiClientFactory;
        private readonly AdministratorFactory _administratorFactory;
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly IOnfidoImposter _onfidoImposter;

        public FileFixture(PublicApiClientFactory publicApiClientFactory,
                           AdministratorFactory administratorFactory,
                           AdminApiClientFactory adminApiClientFactory,
                           IOnfidoImposter onfidoImposter)
        {
            _publicApiClientFactory = publicApiClientFactory ?? throw new ArgumentNullException(nameof(publicApiClientFactory));
            _administratorFactory = administratorFactory ?? throw new ArgumentNullException(nameof(administratorFactory));
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _onfidoImposter = onfidoImposter ?? throw new ArgumentNullException(nameof(onfidoImposter));
        }

        public Task<string[]> UploadAsync(Guid userId,
                                          FileToUpload[] filesToUpload,
                                          ExternalFileProviderType? externalProvider = null,
                                          string externalProfileId = null)
        {
            if (filesToUpload is null)
                throw new ArgumentNullException(nameof(filesToUpload));
            if (externalProvider is not null && externalProfileId is null)
                throw new ArgumentNullException(nameof(externalProfileId));

            return externalProvider switch
            {
                ExternalFileProviderType.Onfido => UploadFileToOnfidoAsync(externalProfileId, filesToUpload),
                null => filesToUpload.Foreach(file => UploadFileToSystemAsync(userId, file)),
                _ => throw new ArgumentOutOfRangeException(nameof(externalProvider))
            };
        }

        public async Task<Guid> UploadByAdminAsync(Guid userId, FileToUpload file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var documentCategory = file.DocumentCategory.To<AdminApi.DocumentCategory>();
            var documentType = file.DocumentType;

            // Act
            var uploadedFile = await client.Documents.UploadAsync(userId, documentCategory, documentType, file.Data.MapToAdmin(), file.UploadToOnfido);
            return uploadedFile.FileId;
        }

        private async Task<string> UploadFileToSystemAsync(Guid userId, FileToUpload file)
        {
            var client = _publicApiClientFactory.Create(userId);
            var documentCategory = file.DocumentCategory.To<DocumentCategory>();
            var result = await client.Documents.UploadAsync(documentCategory, file.DocumentType, file.Data.MapToPublic());
            return result.FileId.ToString();
        }

        private async Task<string[]> UploadFileToOnfidoAsync(string applicantId, FileToUpload[] files)
        {
            var filesToUpload = files.Divide(
                    file => file.DocumentCategory is Models.Enums.DocumentCategory.Selfie && file.DocumentType == DocumentTypes.Photo,
                    file => file.DocumentCategory is Models.Enums.DocumentCategory.Selfie && file.DocumentType == DocumentTypes.Video
                ).Select(file => (id: Guid.NewGuid().ToString(), file.Data))
                 .ToArray();

            if(!filesToUpload.first.IsEmpty())
                await _onfidoImposter.ConfigureDownloadLivePhotosAsync(applicantId, filesToUpload.first);
            if(!filesToUpload.second.IsEmpty())
                await _onfidoImposter.ConfigureDownloadLiveVideosAsync(applicantId, filesToUpload.second);
            if(!filesToUpload.others.IsEmpty())
                await _onfidoImposter.ConfigureDownloadDocumentsAsync(applicantId, filesToUpload.others);

            return filesToUpload.Select(file => file.id).Concat().ToArray();
        }
    }
}
