using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
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
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    [TestFixture]
    internal class DocumentsTests : BaseComponentTest
    {
        private PublicApiClientFactory _publicApiClientFactory;
        private AdministratorFactory _administratorFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private DocumentProvider _documentsProvider;
        private ApplicationFixture _applicationFixture;
        private ExternalProfileFixture _externalProfileFixture;
        private FileFixture _fileFixture;
        private DocumentsFixture _documentsFixture;
        private DocumentStepFixture _documentStepFixture;
        private EventsFixture _eventsFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _documentsProvider = Resolve<DocumentProvider>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _externalProfileFixture = Resolve<ExternalProfileFixture>();
            _fileFixture = Resolve<FileFixture>();
            _documentsFixture = Resolve<DocumentsFixture>();
            _documentStepFixture = Resolve<DocumentStepFixture>();
            _eventsFixture = Resolve<EventsFixture>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<NotGlobalUserInfoArbitrary>();
            Arb.Register<InvalidDocumentTypeArbitrary>();
        }

        /// <summary>
        /// Scenario: User uploads file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// When user uploads file with document category and document type
        /// And document category and document type fit validation rule
        /// And file fit validation rule
        /// And document category fit user action
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);
            var faker = FakerFactory.Create(seed);
            var fileFactory = new FileDataFactory(seed);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var availableCategories = await _documentsProvider.GetAvailableCategoriesAsync(userId);
            var documentCategory = faker.PickRandom(availableCategories).To<DocumentCategory>();

            var validationRules = await client.Validation.GetDocumentRulesAsync(documentCategory);
            var validationRule = faker.PickRandom(validationRules.ValidationRules);

            var documentType = validationRule.DocumentType;
            var fileExtension = faker.PickRandom(validationRule.Extensions);
            var file = fileFactory.Create(documentType, fileExtension);

            // Act
            var result = await client.Documents.UploadAsync(documentCategory, documentType, file.MapToPublic());

            // Assert
            var expectedSteam = file.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(userId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: User uploads file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user already uploaded file
        /// When user uploads other file with document category and document type
        /// And document category and document type equals to already uploaded file
        /// And document category and document type fit validation rule
        /// And file fit validation rule
        /// And document category fit user action
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile_WhenUploadSecondFile(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var initialFile = await _documentsProvider.GetFileToUploadAsync(userId, seed);
            var initialFileId = await _fileFixture.UploadAsync(userId, initialFile);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);
            var fileFactory = new FileDataFactory(seed);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var newFile = fileFactory.Create(initialFile.DocumentType, initialFile.Data.Extension);
            var documentCategory = initialFile.DocumentCategory.To<DocumentCategory>();
            var documentType = initialFile.DocumentType;

            // Act
            var result = await client.Documents.UploadAsync(documentCategory, documentType, newFile.MapToPublic());

            // Assert
            result.FileId.Should().NotBe(initialFileId);

            var expectedSteam = newFile.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(userId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: User uploads same file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have uploaded file
        /// When user uploads same file again with document category and document type
        /// And document category and document type fit validation rule
        /// And file fit validation rule
        /// And document category fit user action
        /// Then he receives fileId of already uploaded file
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetAlreadyUploadedFileId_WhenUserUploadSameFile(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var fileInfo = await _documentsProvider.GetFileToUploadAsync(userId, seed);
            var initialFileId = await _fileFixture.UploadAsync(userId, fileInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);
            var documentCategory = fileInfo.DocumentCategory.To<DocumentCategory>();
            var documentType = fileInfo.DocumentType;

            // Act
            var result = await client.Documents.UploadAsync(documentCategory, documentType, fileInfo.Data.MapToPublic());

            // Assert
            result.FileId.Should().Be(initialFileId);
        }

        /// <summary>
        /// Scenario: User uploads same file
        /// Given user with uploaded file
        /// Given other user with several actions assigned to him
        /// When second uploads valid file with document category and document type
        /// And document category and document type fit validation rule
        /// And file fit validation rule
        /// And document category fit user action
        /// And this file is already uploaded by other user
        /// Then he receives fileId
        /// And file is uploaded
        /// </summary>
        [Theory]
        public async Task ShouldUploadFile_WhenSameFileUploadedByOtherUser(GbUserInfo first, GbUserInfo second, Seed seed)
        {
            // Given
            var firstUserId = first.UserId.Dump();
            var secondUserId = second.UserId.Dump();

            await _applicationFixture.BuildApplicationAsync(first);
            await _applicationFixture.BuildApplicationAsync(second);

            var fileInfo = await _documentsProvider.GetFileToUploadAsync(firstUserId, seed);
            var firstUserFileId = await _fileFixture.UploadAsync(firstUserId, fileInfo);

            // Arrange
            var secondUserClient = _publicApiClientFactory.Create(secondUserId);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var documentCategory = fileInfo.DocumentCategory.To<DocumentCategory>();
            var documentType = fileInfo.DocumentType;

            // Act
            var result = await secondUserClient.Documents.UploadAsync(documentCategory, documentType, fileInfo.Data.MapToPublic());

            // Assert
            result.FileId.Should().NotBe(firstUserFileId);

            var expectedSteam = fileInfo.Data.GetDataCopy();
            var response = await adminApiClient.Files.DownloadWithHttpMessagesAsync(secondUserId, result.FileId);
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualStream = response.Body;
            actualStream.Should().BeEquivalentTo(expectedSteam);
        }

        /// <summary>
        /// Scenario: User uploads invalid file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// When user uploads file
        /// And file size do not fit validation rule
        /// Then he receives error response with status code "Bad Request"
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenFileIsToBig(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);
            var fileFactory = new FileDataFactory(seed);

            var invalidFileSize = ValidationRules.MaxFileSize + 1;
            var fileInfo = await _documentsProvider.GetFileToUploadAsync(userId, seed);
            var file = fileFactory.Create(fileInfo.DocumentType, fileInfo.Data.Extension, invalidFileSize);

            // Act
            Func<Task> func = async () => await client.Documents.UploadAsync((DocumentCategory) fileInfo.DocumentCategory, fileInfo.DocumentType, file.MapToPublic());

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: User uploads invalid file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// When user uploads file
        /// And file extension do not fit validation rule
        /// Then he receives error response with status code "Bad Request"
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenFileHasWrongExtension(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var faker = FakerFactory.Create(seed);
            var fileFactory = new FileDataFactory(seed);
            var client = _publicApiClientFactory.Create(userId);

            var availableCategories = await _documentsProvider.GetAvailableCategoriesAsync(userId);
            var documentCategory = faker.PickRandom(availableCategories).To<DocumentCategory>();
            var validationRules = await client.Validation.GetDocumentRulesAsync(documentCategory);
            var validationRule = faker.PickRandom(validationRules.ValidationRules);

            var documentType = validationRule.DocumentType;
            var fileExtension = faker.PickRandom(ValidationRules.FileExtensions.Where(extension => !validationRule.Extensions.Contains(extension)));
            var file = fileFactory.Create(documentType, fileExtension);

            // Act
            Func<Task> func = async () => await client.Documents.UploadAsync(documentCategory, documentType, file.MapToPublic());

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: User uploads file
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// When user uploads file with document category and document type
        /// And document category and document type do not fit validation rule
        /// Then he receives error response with status code "Bad Request"
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUploadFileWithInvalidType(UserInfo userInfo, InvalidDocumentType documentType, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);

            var fileToUpload = await _documentsProvider.GetFileToUploadAsync(userId, seed);
            var documentCategory = fileToUpload.DocumentCategory.To<DocumentCategory>();

            // Act
            Func<Task> func = async () => await client.Documents.UploadAsync(documentCategory, documentType, fileToUpload.Data.MapToPublic());

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: User uploads file for closed action
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have submitted document
        /// When user uploads file with document category and document type
        /// And document category equals submitted document
        /// Then he receives error response with status code "Bad Request"
        /// And file is not uploaded
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenActionIsClosed(NotGlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed);
            await _documentStepFixture.CompleteAsync(userId, documentToSubmit.DocumentCategory, seed);

            // Arrange
            var documentCategory = documentToSubmit.DocumentCategory.To<DocumentCategory>();
            var documentType = documentToSubmit.DocumentType;
            var file = documentToSubmit.Files.First();

            // Act
            Func<Task> func = async () => await client.Documents.UploadAsync(documentCategory, documentType, file.MapToPublic());

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: User submits document
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have uploaded several files
        /// When user submits document with fileIds and document category and document type
        /// And document category and document type fit validation rule
        /// And file quantity fit validation rule
        /// And document category fit user action
        /// Then document is submitted
        /// And DocumentSubmitted event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSubmitDocument(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed);
            var fileIds = await _documentsFixture.UploadFilesAsync(userId, documentToSubmit);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var request = new SubmitDocumentRequest
            {
                Category = documentToSubmit.DocumentCategory.To<DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = fileIds
            };

            // Act
            await client.Documents.SubmitAsync(request);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(correlationId);
            var documentId = @event.EventArgs.DocumentId;

            var uploadedDocument = await adminApiClient.Documents.GetAsync(userId, documentId);
            using (new AssertionScope())
            {
                uploadedDocument.Category.Should().Be(documentToSubmit.DocumentCategory.To<AdminApi.DocumentCategory>());
                uploadedDocument.Type.Should().Be(documentToSubmit.DocumentType);
                uploadedDocument.SubmittedAt.Should().NotBe(default);
                uploadedDocument.Files.Should().HaveSameCount(documentToSubmit.Files);
            }
        }

        /// <summary>
        /// Scenario: User submits document
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have several files uploaded to external provider
        /// When user submits document with fileIds and external provider and document category and document type
        /// And document category and document type fit validation rule
        /// And file quantity fit validation rule
        /// And document category and document type fit user action
        /// Then external provider files is uploaded to system
        /// And document is submitted
        /// And DocumentSubmitted event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSubmitDocument_WhenFilesAreUploadedToExternalProvider(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed);
            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            documentToSubmit.SetExternalProvider(ExternalFileProviderType.Onfido, applicantId);

            var fileIds = await _documentsFixture.UploadFilesAsync(userId, documentToSubmit);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var request = new SubmitDocumentRequest
            {
                Category = documentToSubmit.DocumentCategory.To<DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = fileIds,
                Provider = documentToSubmit.ExternalProviderType
            };

            // Act
            await client.Documents.SubmitAsync(request);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<DocumentSubmittedEvent>(correlationId);
            var documentId = @event.EventArgs.DocumentId;

            var uploadedDocument = await adminApiClient.Documents.GetAsync(userId, documentId);
            using (new AssertionScope())
            {
                uploadedDocument.Category.Should().Be(documentToSubmit.DocumentCategory.To<AdminApi.DocumentCategory>());
                uploadedDocument.Type.Should().Be(documentToSubmit.DocumentType);
                uploadedDocument.SubmittedAt.Should().NotBe(default);
                uploadedDocument.Files.Should().HaveSameCount(documentToSubmit.Files);
            }
        }

        /// <summary>
        /// Scenario: User submits document
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have uploaded several files
        /// When user submits document with fileIds and document category and document type
        /// And document type do not fit document category
        /// Then he receives error response with status code "Bad Request"
        /// And document is not submitted
        /// And DocumentSubmitted event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenSubmitDocumentWithInvalidType(UserInfo userInfo, InvalidDocumentType documentType, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed);
            var fileIds = await _documentsFixture.UploadFilesAsync(userId, documentToSubmit);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var request = new SubmitDocumentRequest
            {
                Category = documentToSubmit.DocumentCategory.To<DocumentCategory>(),
                Type = documentType,
                Files = fileIds
            };

            // Act
            Func<Task> func = async () => await client.Documents.SubmitAsync(request);

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _eventsFixture.ShouldNotExist<DocumentSubmittedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User submits document
        /// Given user with several actions assigned to him
        /// And some actions represent documents
        /// And user have uploaded several files
        /// When user submits document with fileIds and document category and document type
        /// And file quantity do not fit validation rule
        /// Then he receives error response with status code "Bad Request"
        /// And document is not submitted
        /// And DocumentSubmitted event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenFileQuantityDoNotFitValidationRule(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var faker = FakerFactory.Create(seed);
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);

            var availableCategories = await _documentsProvider.GetAvailableCategoriesAsync(userId);
            var documentCategory = faker.PickRandom(availableCategories).To<DocumentCategory>();

            var validationRules = await client.Validation.GetDocumentRulesAsync(documentCategory);
            var validationRule = faker.PickRandom(validationRules.ValidationRules);

            var invalidFileQuantity = DocumentProvider.GetInvalidFilesQuantity(validationRule, seed);
            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed, invalidFileQuantity);
            var fileIds = await _documentsFixture.UploadFilesAsync(userId, documentToSubmit);

            var request = new SubmitDocumentRequest
            {
                Category = documentCategory,
                Type = validationRule.DocumentType,
                Files = fileIds
            };

            // Act
            Func<Task> func = async () => await client.Documents.SubmitAsync(request);

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _eventsFixture.ShouldNotExist<DocumentSubmittedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: User submits document
        /// Given user with several actions assigned to him
        /// And user have submitted document
        /// When user submits document with fileIds and document category and document type
        /// And document category equals submitted document
        /// Then he receives error response with status code "Bad Request"
        /// And document is not submitted
        /// And DocumentSubmitted event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenDocumentAlreadySubmitted(NotGlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var documentToSubmit = await _documentsProvider.GetDocumentToSubmitAsync(userId, seed);
            await _documentStepFixture.CompleteAsync(userId, documentToSubmit.DocumentCategory, seed);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            var submittedDocument = (await adminApiClient.Documents.GetAllAsync(userId)).First();
            var request = new SubmitDocumentRequest
            {
                Category = documentToSubmit.DocumentCategory.To<DocumentCategory>(),
                Type = documentToSubmit.DocumentType,
                Files = submittedDocument.Files.Select(file => file.Id.ToString()).ToArray()
            };

            // Act
            Func<Task> func = async () => await client.Documents.SubmitAsync(request);

            // Assert
            var errorResponse = await func.Should().ThrowAsync<ErrorResponseException>();
            errorResponse.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _eventsFixture.ShouldNotExist<DocumentSubmittedEvent>(correlationId);
        }
    }
}
