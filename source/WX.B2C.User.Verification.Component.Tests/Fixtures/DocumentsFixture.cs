using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using PublicApi = WX.B2C.User.Verification.Api.Public.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class DocumentsFixture
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly PublicApiClientFactory _publicApiClientFactory;
        private readonly DocumentProvider _documentProvider;
        private readonly FileFixture _fileFixture;
        private readonly EventsFixture _eventsFixture;

        public DocumentsFixture(AdminApiClientFactory adminApiClientFactory,
                                AdministratorFactory adminFactory,
                                PublicApiClientFactory publicApiClientFactory,
                                DocumentProvider documentProvider,
                                FileFixture fileFixture,
                                EventsFixture eventsFixture)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _publicApiClientFactory = publicApiClientFactory ?? throw new ArgumentNullException(nameof(publicApiClientFactory));
            _documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
            _fileFixture = fileFixture ?? throw new ArgumentNullException(nameof(fileFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public Task<string[]> UploadFilesAsync(Guid userId, DocumentToSubmit documentToSubmit)
        {
            if (documentToSubmit is null)
                throw new ArgumentNullException(nameof(documentToSubmit));

            var documentCategory = documentToSubmit.DocumentCategory;
            var documentType = documentToSubmit.DocumentType;
            var externalProvider = documentToSubmit.ExternalProviderType;
            var externalProfileId = documentToSubmit.ExternalProfileId;
            var filesToUpload = documentToSubmit.Files.Select(Map).ToArray();

            return _fileFixture.UploadAsync(userId, filesToUpload, externalProvider, externalProfileId);

            FileToUpload Map(FileData file) => file.Map(documentCategory, documentType);
        }

        public async Task<Guid> SubmitAsync(Guid userId, DocumentCategory documentCategory, Seed seed, Guid? correlationId = null)
        {
            if (seed is null)
                throw new ArgumentNullException(nameof(seed));

            correlationId ??= Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);

            var documentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, documentCategory, seed);
            var filesIds = await UploadFilesAsync(userId, documentToSubmit);
            await client.Documents.SubmitAsync(new PublicApi.SubmitDocumentRequest
            {
                Category = documentCategory.To<PublicApi.DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = filesIds,
                Provider = documentToSubmit.ExternalProviderType
            });

            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(correlationId.Value);
            return @event.EventArgs.DocumentId;
        }

        public async Task<Guid> SubmitAsync(Guid userId, DocumentToSubmit documentToSubmit)
        {
            if (documentToSubmit is null)
                throw new ArgumentNullException(nameof(documentToSubmit));

            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);

            var filesIds = await UploadFilesAsync(userId, documentToSubmit);
            await client.Documents.SubmitAsync(new PublicApi.SubmitDocumentRequest
            {
                Category = documentToSubmit.DocumentCategory.To<PublicApi.DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = filesIds,
                Provider = documentToSubmit.ExternalProviderType
            });

            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(correlationId);
            return @event.EventArgs.DocumentId;
        }

        public async Task<Guid> SubmitByAdminAsync(Guid userId, DocumentToSubmit documentToSubmit, Guid? correlationId = null)
        {
            if (documentToSubmit is null)
                throw new ArgumentNullException(nameof(documentToSubmit));

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            var documentCategory = documentToSubmit.DocumentCategory;
            var documentType = documentToSubmit.DocumentType;
            var filesToUpload = documentToSubmit.Files.Select(file => file.Map(documentCategory, documentType));

            var filesIds = await filesToUpload.Foreach(file => _fileFixture.UploadByAdminAsync(userId, file));

            await client.Documents.SubmitAsync(new AdminApi.SubmitDocumentRequest
            {
                Category = documentCategory.To<AdminApi.DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = filesIds.Select(id => id.ToString()).ToArray(),
                Reason = nameof(DocumentsFixture)
            }, userId);

            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(client.CorrelationId);
            return @event.EventArgs.DocumentId;
        }
    }
}
