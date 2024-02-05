using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using Serilog.Core;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.Configuration.Contracts;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    public class PassFortClientTests : BaseIntegrationTest
    {
        private IPassFortApiClientFactory _clientFactory;
        private IPassFortApiClient _client;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(context => new SelfHostingValuesResolver("PassFort", "KeyVault"))
                            .As<IHostingSpecificValuesResolver>();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Arb.Register<PassFortProfileArbitrary>();
        }

        [SetUp]
        public void SetUp()
        {
            var valueResolver = Resolve<IHostingSpecificValuesResolver>();
            var settings = new PassFortApiSettings
            {
                ApiKey = valueResolver.GetValue("ApiKey"),
                ApiUri = new Uri(valueResolver.GetValue("ApiUri"))
            };

            _clientFactory = new PassFortApiClientFactory(settings, new PassFortPolicyFactory(Logger.None));
            _client = _clientFactory.Create();
        }

        [Theory]
        public async Task ShouldCreateProfile(PassFortProfile newProfile)
        {
            // Act
            var result = await _client.Profiles.CreateWithHttpMessagesAsync(newProfile);

            // Arrange
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var profileId = result.Body.Id;
            var profile = await _client.Profiles.GetAsync(profileId);
            profile.Role.Should().Be("INDIVIDUAL_CUSTOMER");
            profile.Applications.Count.Should().Be(1);
            profile.Applications.First().Status.Should().Be("APPLIED");
        }

        [Theory]
        public async Task ShouldUpdateCollectedData(PassFortProfile newProfile, NonEmptyString newFamilyName)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var collectedData = newProfile.CollectedData as IndividualData;

            var response = await _client.Profiles.GetCollectedDataWithHttpMessagesAsync(profile.Id);
            var etag = response.Headers.Etag;

            // Arrange
            collectedData.PersonalDetails.Name.FamilyName = newFamilyName.Get;

            // Act
            var result = await _client.Profiles.UpdateCollectedDataWithHttpMessagesAsync(profile.Id, collectedData, etag);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var actualCollectedData = await _client.Profiles.GetCollectedDataAsync(profile.Id);
            actualCollectedData.PersonalDetails.Name.FamilyName.Should().Be(newFamilyName.Get);
        }

        [Theory]
        public async Task ShouldUpdateAddressInCollectedData(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var collectedData = newProfile.CollectedData as IndividualData;

            var response = await _client.Profiles.GetCollectedDataWithHttpMessagesAsync(profile.Id);
            var etag = response.Headers.Etag;

            // Arrange
            var freeformAddress = new FreeformAddress("CHN", "Test, 3");
            collectedData.AddressHistory = new List<DatedAddressHistoryItem>() { new(freeformAddress) };

            // Act
            var result = await _client.Profiles.UpdateCollectedDataWithHttpMessagesAsync(profile.Id, collectedData, etag);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var operationResponse = await _client.Profiles.GetCollectedDataAsync(profile.Id);

            operationResponse.AddressHistory.Should().HaveCount(1);
            operationResponse.AddressHistory.First().Address.Should().BeOfType<StructuredAddress>();
        }
        
        [Theory]
        public async Task ShouldUpdateApplicationState(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var application = profile.Applications.First();

            // Arrange
            var newState = "REJECTED";
            var request = new ProductApplication { Status = newState };

            // Act
            await _client.Applications.UpdateWithHttpMessagesAsync(profile.Id, application.Id, request);

            // Assert
            application = await _client.Applications.GetAsync(profile.Id, application.Id);
            application.Status.Should().Be(newState);
        }

        [Theory]
        public async Task ShouldRevertApplicationDecision(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var application = profile.Applications.First();
            await _client.Applications.UpdateAsync(profile.Id, application.Id, new ProductApplication { Status = "REJECTED" });

            // Act
            var result = await _client.Applications.RevertDecisionWithHttpMessagesAsync(profile.Id, application.Id);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            application = await _client.Applications.GetAsync(profile.Id, application.Id);
            application.Status.Should().Be("APPLIED");
        }

        [Theory]
        public async Task ShouldCreateTask(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            // Arrange
            var request = new TaskRequest
            {
                Type = TaskType.INDIVIDUALMANUALTASK,
            };

            // Act
            var result = await _client.Tasks.AddWithHttpMessagesAsync(profile.Id, request);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var taskId = result.Body.Id;

            var getTaskResponse = await _client.Tasks.GetWithHttpMessagesAsync(profile.Id, taskId);
            getTaskResponse.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        public async Task ShouldCompleteTask(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var task = profile.Tasks.First();

            // Assert
            var result = await _client.Tasks.CompleteWithHttpMessagesAsync(profile.Id, task.Id, new TaskResource { IsComplete = true });

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            task = await _client.Tasks.GetAsync(profile.Id, task.Id);
            task.IsComplete.Should().Be(true);
        }

        [Theory]
        public async Task ShouldUploadDocumentImage(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            // Arrange
            var documentImage = new DocumentImageResource
            {
                ImageType = DocumentImageType.FRONT,
                Data =
                    "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="
            };

            // Act
            var result = await _client.Documents.UploadWithHttpMessagesAsync(profile.Id, documentImage);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var documentImages = await _client.Documents.ListAsync(profile.Id);
            documentImages.Count.Should().Be(1);
            documentImages.First().Id.Should().Be(result.Body.Id);
        }

        [Theory]
        public async Task ShouldAddDocument(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            var documentImage = new DocumentImageResource
            {
                ImageType = DocumentImageType.FRONT,
                Data =
                    "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="
            };
            var uploadedDocumentImage = await _client.Documents.UploadAsync(profile.Id, documentImage);

            // Arrange
            var documentPost = new DocumentPost
            {
                Category = DocumentCategory.PROOFOFIDENTITY,
                DocumentType = DocumentType.PASSPORT,
                Images = new List<DocumentPostImagesItem>
                {
                    new DocumentPostImagesItem
                    {
                        Id = uploadedDocumentImage.Id
                    }
                }
            };

            // Act
            var result = await _client.Profiles.AddDocumentsWithHttpMessagesAsync(profile.Id, documentPost);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        public async Task ShouldAddTag(PassFortProfile newProfile, NonEmptyString tagName)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            // Act
            var result = await _client.Tags.AddWithHttpMessagesAsync(profile.Id, new TagResource { Name = tagName.Get });

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            profile = await _client.Profiles.GetAsync(profile.Id);
            profile.Tags.Count.Should().Be(1);
            profile.Tags.First().Name.Should().Be(tagName.Get);
        }

        [Theory]
        public async Task ShouldRemoveTag(PassFortProfile newProfile, NonEmptyString tagName)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);
            var createdTag = await _client.Tags.AddAsync(profile.Id, new TagResource { Name = tagName.Get });

            // Act
            var result = await _client.Tags.DeleteWithHttpMessagesAsync(profile.Id, createdTag.Id);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            profile = await _client.Profiles.GetAsync(profile.Id);
            profile.Tags.Should().BeEmpty();
        }

        [Theory]
        public async Task ShouldRunCheckAsync(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            // Arrange
            var request = new CheckRequest
            {
                CheckType = CheckType.IDENTITYCHECK
            };

            // Act
            var result = await _client.Checks.RunWithHttpMessagesAsync(profile.Id, request, mode: "async");

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var check = await _client.Checks.GetAsync(profile.Id, result.Body.Id);
            check.Should().NotBeNull();
            check.TaskIds.Should().NotBeNullOrEmpty();
        }

        [Theory]
        public async Task ShouldRunCheckSync(PassFortProfile newProfile)
        {
            // Given
            var profile = await _client.Profiles.CreateAsync(newProfile);

            // Arrange
            var request = new CheckRequest
            {
                CheckType = CheckType.IDENTITYCHECK
            };

            // Act
            var result = await _client.Checks.RunWithHttpMessagesAsync(profile.Id, request, mode: "sync");

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var check = await _client.Checks.GetAsync(profile.Id, result.Body.Id);
            check.Should().NotBeNull();
            check.State.Should().Be(CheckState.COMPLETE);
            check.TaskIds.Should().NotBeNullOrEmpty();
        }
        
        [Theory, Ignore("Only for manual testing as exceeds passfort rate limit")]
        public async Task ShouldCreateProfile_WhenRateLimitExceeded(PassFortProfile newProfile)
        {
            // Additional guard to avoid useless run
            if (!Debugger.IsAttached)
                throw new InvalidOperationException("This test must be used only for testing input from PassFort");
            
            var requestsNumber = 1000;
            var tasks = new Task[requestsNumber];
            
            // Act
            for (int i = 0; i < requestsNumber; i++)
            {
                tasks[i] = _client.Profiles.CreateWithHttpMessagesAsync(newProfile);
            }
            var act = new Func<Task>(() => tasks.WhenAll());
            
            // Arrange
            await act.Should().NotThrowAsync();
        }
    }
}