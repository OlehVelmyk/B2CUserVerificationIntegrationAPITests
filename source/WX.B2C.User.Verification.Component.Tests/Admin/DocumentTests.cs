using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using FileToUpload = WX.B2C.User.Verification.Component.Tests.Models.FileToUpload;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    [TestFixture]
    internal class DocumentTests : BaseComponentTest
    {
        private AdministratorFactory _administratorFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private DocumentProvider _documentProvider;
        private ApplicationFixture _applicationFixture;
        private FileFixture _fileFixture;
        private DocumentsFixture _documentsFixture;
        private EventsFixture _eventsFixture;
        private DbFixture _dbFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _administratorFactory = Resolve<AdministratorFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _documentProvider = Resolve<DocumentProvider>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _fileFixture = Resolve<FileFixture>();
            _documentsFixture = Resolve<DocumentsFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _dbFixture = Resolve<DbFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<FileToUploadArbitrary>();
            Arb.Register<FileToUploadArrayArbitrary>();
        }

        /// <summary>
        /// Scenario: Admin requests user documents by userId
        /// Given user with several documents submitted
        /// When admin requests user documents by userId
        /// Then he receives all user documents
        /// </summary>
        [Theory]
        public async Task ShouldGetAllUserDocuments(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var availableCategories = await _documentProvider.GetAvailableCategoriesAsync(userId);
            var firstDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[0], seed);
            var firstDocumentId = await _documentsFixture.SubmitAsync(userId, firstDocumentToSubmit);
            var secondDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[1], seed);
            var secondDocumentId = await _documentsFixture.SubmitAsync(userId, secondDocumentToSubmit);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var firstDocumentCategory = firstDocumentToSubmit.DocumentCategory.To<DocumentCategory>();
            var secondDocumentCategory = secondDocumentToSubmit.DocumentCategory.To<DocumentCategory>();

            // Act
            var documents = await client.Documents.GetAllAsync(userId);

            // Assert
            documents.Should().HaveCount(2);
            var firstDocument = documents.First(d => d.Category == firstDocumentCategory);
            var secondDocument = documents.First(d => d.Category == secondDocumentCategory);
            using (new AssertionScope())
            {
                firstDocument.Should().NotBe(null, $"Document with category {firstDocumentCategory} not found");
                firstDocument.Id.Should().Be(firstDocumentId);
                firstDocument.Type.Should().Be(firstDocumentToSubmit.DocumentType);
                firstDocument.Files.Should().HaveSameCount(firstDocumentToSubmit.Files);

                secondDocument.Should().NotBe(null, $"Document with category {secondDocumentCategory} not found");
                secondDocument.Id.Should().Be(secondDocumentId);
                secondDocument.Type.Should().Be(secondDocumentToSubmit.DocumentType);
                secondDocument.Files.Should().HaveSameCount(secondDocumentToSubmit.Files);
            }
        }

        /// <summary>
        /// Scenario: Admin requests user documents by userId
        /// Given user without documents
        /// When admin requests user documents by userId
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollections_WhenUserDoNotHaveDocuments(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var documents = await client.Documents.GetAllAsync(userId);

            // Assert
            documents.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Admin requests user documents by userId and document category
        /// Given user with several documents submitted
        /// When admin requests user documents by userId and document category
        /// Then he receive documents
        /// And documents category equals to requested
        /// </summary>
        [Theory]
        public async Task ShouldGetUserDocumentsByDocumentCategory(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var availableCategories = await _documentProvider.GetAvailableCategoriesAsync(userId);
            var firstDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[0], seed);
            await _documentsFixture.SubmitAsync(userId, firstDocumentToSubmit);

            var secondDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[1], seed);
            var secondDocumentId = await _documentsFixture.SubmitAsync(userId, secondDocumentToSubmit);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var searchCategory = secondDocumentToSubmit.DocumentCategory.To<DocumentCategory>();

            // Act
            var documents = await client.Documents.GetAllAsync(userId, searchCategory);

            // Assert
            documents.Should().HaveCount(1);
            var document = documents.First();
            using (new AssertionScope())
            {
                document.Id.Should().Be(secondDocumentId);
                document.Category.Should().Be(searchCategory);
                document.Type.Should().Be(secondDocumentToSubmit.DocumentType);
                document.Files.Should().HaveSameCount(secondDocumentToSubmit.Files);
            }
        }

        /// <summary>
        /// Scenario: Admin requests user documents by userId and document category
        /// Given user with several documents submitted
        /// And user do not have document with such category
        /// When admin requests user documents by userId and document category
        /// And no documents with such category exists
        /// Then he receives empty collections
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollections_WhenUserDoNotHaveDocumentsWithSuchCategory(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var availableCategories = await _documentProvider.GetAvailableCategoriesAsync(userId);
            var firstDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[0], seed);
            await _documentsFixture.SubmitAsync(userId, firstDocumentToSubmit);

            var secondDocumentToSubmit = await _documentProvider.GetDocumentToSubmitAsync(userId, availableCategories[1], seed);
            await _documentsFixture.SubmitAsync(userId, secondDocumentToSubmit);

            // Arrange
            var faker = FakerFactory.Create(seed);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var firstDocumentCategory = firstDocumentToSubmit.DocumentCategory.To<DocumentCategory>();
            var secondDocumentCategory = secondDocumentToSubmit.DocumentCategory.To<DocumentCategory>();
            var searchCategory = faker.Random.Enum(firstDocumentCategory, secondDocumentCategory);

            // Act
            var documents = await client.Documents.GetAllAsync(userId, searchCategory);

            // Assert
            documents.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Admin requests user file by userId and fileId
        /// Given user with uploaded file
        /// When admin requests user file by userId and fileId
        /// Then he receives it
        /// </summary>
        [Theory]
        public async Task ShouldGetFile(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var fileToUpload = await _documentProvider.GetFileToUploadAsync(userId, seed);
            var fileId = Guid.Parse(await _fileFixture.UploadAsync(userId, fileToUpload));

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var result = await client.Files.DownloadWithHttpMessagesAsync(userId, fileId);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Body.Should().BeEquivalentTo(fileToUpload.Data.GetDataCopy());
        }

        /// <summary>
        /// Scenario: Admin requests user file by userId and fileId
        /// Given user with several uploaded files
        /// When admin requests user file by userId and dummy fileId
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenFileNotExists(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var fileToUpload = await _documentProvider.GetFileToUploadAsync(userId, seed);
            await _fileFixture.UploadAsync(userId, fileToUpload);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var fileId = Guid.NewGuid();

            // Act
            Func<Task> downloadFile = () => client.Files.DownloadWithHttpMessagesAsync(userId, fileId);

            // Assert
            var exception = await downloadFile.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Admin uploads file
        /// Given user
        /// When admin uploads file with document category and document type
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile(UserInfo userInfo, FileToUpload fileToUpload)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var documentCategory = fileToUpload.DocumentCategory.To<DocumentCategory>();
            var documentType = fileToUpload.DocumentType;

            // Act
            var result = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin());

            // Assert
            var expectedSteam = fileToUpload.Data.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(userId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: Admin uploads file
        /// Given user with several files uploaded
        /// When admin uploads other file with document category and document type
        /// And document category and document type equals to already uploaded file
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile_WhenOtherFilesIsUploaded(UserInfo userInfo, FileToUpload[] filesToUpload)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var initialFiles = filesToUpload[..^1];
            var fileIds = await Task.WhenAll(initialFiles.Select(file => _fileFixture.UploadByAdminAsync(userId, file)));

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var newFile = filesToUpload[^1];
            var documentCategory = newFile.DocumentCategory.To<DocumentCategory>();
            var documentType = newFile.DocumentType;

            // Act
            var result = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, newFile.Data.MapToAdmin());

            // Assert
            fileIds.Should().NotContain(result.FileId);

            var expectedSteam = newFile.Data.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(userId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: Admin uploads file
        /// Given user with uploaded file
        /// When admin uploads same file again with document category and document type
        /// Then he receives fileId of already uploaded file
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetAlreadyUploadedFileId_WhenUploadSameFile(UserInfo userInfo, FileToUpload fileToUpload)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var fileId = await _fileFixture.UploadByAdminAsync(userId, fileToUpload);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var documentCategory = fileToUpload.DocumentCategory.To<DocumentCategory>();
            var documentType = fileToUpload.DocumentType;

            // Act
            var result = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin());

            // Assert
            result.FileId.Should().Be(fileId);
        }

        /// <summary>
        /// Scenario: Admin uploads file
        /// Given user
        /// And other user with uploaded file
        /// When admin uploads file with document category and document type
        /// And this file is already uploaded by other user
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile_WhenSameFileUploadedByOtherUser(UserInfo first, UserInfo second, FileToUpload fileToUpload)
        {
            // Given
            var firstUserId = first.UserId.Dump();
            var secondUserId = second.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(first);
            await _applicationFixture.BuildApplicationAsync(second);

            var fileId = await _fileFixture.UploadByAdminAsync(firstUserId, fileToUpload);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var documentCategory = fileToUpload.DocumentCategory.To<DocumentCategory>();
            var documentType = fileToUpload.DocumentType;

            // Act
            var result = await adminApiClient.Documents.UploadAsync(secondUserId, documentCategory, documentType, fileToUpload.Data.MapToAdmin());

            // Assert
            result.FileId.Should().NotBe(fileId);

            var expectedSteam = fileToUpload.Data.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(secondUserId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: Admin uploads file to external provider
        /// Given user
        /// When admin uploads file with document category and document type
        /// And he selected to upload file to external provider
        /// Then file is successfully uploaded to system and external provider
        /// And file has external id
        /// </summary>
        [Theory]
        public async Task ShouldUploadFileToProvider(UserInfo userInfo, FileToUpload fileToUpload)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var documentCategory = DocumentCategory.Selfie;
            var documentType = DocumentTypes.Photo;

            // Act
            var file = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true);

            // Assert
            var externalId = await _dbFixture.FindExternalId(file.FileId);
            externalId.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin does not upload file to external provider
        /// Given user
        /// When admin uploads file with document category and document type
        /// And he selected not to upload file to external provider
        /// Then file is successfully uploaded to system
        /// And file is not uploaded to external provider
        /// And file has no external id
        /// </summary>
        [Theory]
        public async Task ShouldNotUploadFileToProvider(UserInfo userInfo, FileToUpload fileToUpload)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var documentCategory = DocumentCategory.Supporting;
            var documentType = DocumentTypes.Other;

            // Act
            var file = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), false);

            // Assert
            var externalId = await _dbFixture.FindExternalId(file.FileId);
            externalId.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Admin re-uploads file to external provider
        /// Given user with uploaded file
        /// When admin uploads same file
        /// And selected to upload file to external provider
        /// Then file is not duplicated in system
        /// And file is uploaded to external provider
        /// And previously created file got external id
        /// </summary>
        [Theory]
        public async Task ShouldUploadFileToProvider_WhenExternalIdNotExists(UserInfo userInfo, FileToUpload fileToUpload, Seed seed)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var documentCategory = DocumentCategory.ProofOfIdentity;
            var documentType = FakerFactory.Create(seed).PickRandom(DocumentTypes.IdentityDocumentTypes);

            var expectedFile = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), false);
            var previousExternalId = await _dbFixture.FindExternalId(expectedFile.FileId);

            // Act
            var actualFile = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true);

            // Assert
            var actualExternalId = await _dbFixture.FindExternalId(actualFile.FileId);
            actualFile.FileId.Should().Be(expectedFile.FileId);
            previousExternalId.Should().BeNull();
            actualExternalId.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin re-uploads file to external provider
        /// Given user with uploaded file
        /// And file already uploaded to external provider
        /// When admin uploads same file
        /// And selected to upload file to external provider again
        /// Then file is not duplicated in system
        /// And file is not uploaded to external provider
        /// And external id is not changed
        /// </summary>
        [Theory]
        public async Task ShouldNotUploadFileToProvider_WhenExternalIdExists(UserInfo userInfo, FileToUpload fileToUpload, Seed seed)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var documentCategory = DocumentCategory.ProofOfIdentity;
            var documentType = FakerFactory.Create(seed).PickRandom(DocumentTypes.IdentityDocumentTypes);

            var expectedFile = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true);
            var expectedExternalId = await _dbFixture.FindExternalId(expectedFile.FileId);

            // Act
            var actualFile = await adminApiClient.Documents.UploadAsync(userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true);

            // Assert
            var actualExternalId = await _dbFixture.FindExternalId(actualFile.FileId);
            actualFile.FileId.Should().Be(expectedFile.FileId);
            actualExternalId.Should().Be(expectedExternalId);
        }

        /// <summary>
        /// Scenario: Admin uploads video to Onfido
        /// Given user
        /// When admin uploads video
        /// And selected to upload file to Onfido
        /// Then he receives error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldThrowException_WhenUploadVideoToOnfido(UserInfo userInfo, FileToUpload fileToUpload)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Arrange
            var documentCategory = DocumentCategory.Selfie;
            var documentType = DocumentTypes.Video;
            var externalProvider = ExternalProviderType.Onfido;

            // Act
            Func<Task> getApplication = () => adminApiClient.Documents.UploadAsync(
                userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true, externalProvider);

            // Assert
            var exception = await getApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Admin uploads unsupported file to external provider
        /// Given user
        /// When admin uploads file
        /// And selected external provider does not support specified file
        /// Then he receives error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldThrowException_WhenUploadUnsupportedFileToProvider(UserInfo userInfo, FileToUpload fileToUpload, Seed seed)
        {
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Arrange
            var documentCategory = DocumentCategory.ProofOfAddress;
            var documentType = FakerFactory.Create(seed).PickRandom(DocumentTypes.AddressDocumentTypes);
            var externalProvider = ExternalProviderType.Onfido;

            // Act
            Func<Task> getApplication = () => adminApiClient.Documents.UploadAsync(
                userId, documentCategory, documentType, fileToUpload.Data.MapToAdmin(), true, externalProvider);

            // Assert
            var exception = await getApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Admin submits document
        /// Given user with several files uploaded
        /// When admin submits document with fileIds and document category and document type
        /// Then document is submitted
        /// And DocumentSubmitted event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSubmitDocument(UserInfo userInfo, FileToUpload[] filesToUpload)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var fileIds = await Task.WhenAll(filesToUpload.Select(file => _fileFixture.UploadByAdminAsync(userId, file)));

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            var request = new SubmitDocumentRequest
            {
                Reason = nameof(ShouldSubmitDocument),
                Category = filesToUpload.First().DocumentCategory.To<DocumentCategory>(),
                Type = filesToUpload.First().DocumentType,
                Files = fileIds.Select(id => id.ToString()).ToArray()
            };

            // Act
            await client.Documents.SubmitAsync(request, userId);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(correlationId);
            var documentId = @event.EventArgs.DocumentId;

            var uploadedDocument = await client.Documents.GetAsync(userId, documentId);
            using (new AssertionScope())
            {
                uploadedDocument.Category.Should().Be(request.Category.To<DocumentCategory>());
                uploadedDocument.Type.Should().Be(request.Type);
                uploadedDocument.SubmittedAt.Should().NotBe(default);
                uploadedDocument.Files.Should().HaveSameCount(filesToUpload);
            }
        }
    }
}
