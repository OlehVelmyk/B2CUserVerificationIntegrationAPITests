using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    public class OnfidoClientTests : BaseIntegrationTest
    {
        private readonly List<string> applicantIds = new List<string>();
        private IOnfidoApiClient _onfidoApiClient;
        private OnfidoTokenFixture _onfidoTokenFixture;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterOnfidoClient();
            containerBuilder.RegisterType<OnfidoTokenFixture>().SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _onfidoTokenFixture = Resolve<OnfidoTokenFixture>();

            _onfidoApiClient = Resolve<IOnfidoApiClientFactory>().Create();

            Arb.Register<OnfidoApplicantArbitrary>();
            Arb.Register<OnfidoApplicantWithForbiddenCharactersArbitrary>();
            Arb.Register<ReferrerArbitrary>();
            Arb.Register<InvalidReferrerArbitrary>();
            Arb.Register<DocumentContainerArbitrary>();
            Arb.Register<LivePhotoContainerArbitrary>();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            var tasks = applicantIds.Select(id => _onfidoApiClient.Applicants.DestroyAsync(id));
            applicantIds.Clear();
            await Task.WhenAll(tasks);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _onfidoApiClient.Dispose();
        }

        [Theory(MaxTest = 2)]
        public async Task ShouldCreateApplicant(NewApplicant applicant)
        {
            // Act
            var result = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(result.Id);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be(applicant.FirstName);
            result.LastName.Should().Be(applicant.LastName);
        }

        [Theory]
        public async Task ShouldThrowException_WhenCreateApplicantWithForbiddenCharacters(OnfidoApplicantWithForbiddenCharacters applicant)
        {
            // Act
            Func<Task> result = async () => await _onfidoApiClient.Applicants.CreateAsync(applicant);

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }

        [Theory(MaxTest = 2)]
        public async Task ShouldCreateMobileToken_WhenApplicantExist(NewApplicant applicant)
        {
            // Arrange 
            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(onfidoApplicant.Id);
            var token = _onfidoTokenFixture.Create(onfidoApplicant.Id);

            // Act
            var result = await _onfidoApiClient.SdkToken.GenerateAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNull();
        }

        [Theory(MaxTest = 2)]
        public async Task ShouldCreateWebToken_WhenApplicantExist(NewApplicant applicant, Referrer referrer)
        {
            // Arrange 
            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(onfidoApplicant.Id);
            var token = _onfidoTokenFixture.Create(onfidoApplicant.Id, referrer.Value);

            // Act
            var result = await _onfidoApiClient.SdkToken.GenerateAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNull();
        }

        [Theory]
        public async Task ShouldThrowException_WhenCreateWebTokenWithIncorrectReferrer(NewApplicant applicant, InvalidReferrer referrer)
        {
            // Arrange 
            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(onfidoApplicant.Id);
            var token = _onfidoTokenFixture.Create(onfidoApplicant.Id, referrer.Value);

            // Act
            Func<Task> result = async () => await _onfidoApiClient.SdkToken.GenerateAsync(token);

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }

        [Theory]
        public async Task ShouldThrowException_WhenCreateMobileTokenWhenApplicantNotExist(Guid applicantId)
        {
            // Arrange
            var token = _onfidoTokenFixture.Create(applicantId.ToString());

            // Act
            Func<Task> result = async () => await _onfidoApiClient.SdkToken.GenerateAsync(token);

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }

        [Theory]
        public async Task ShouldThrowException_WhenCreateWebTokenWhenApplicantNotExist(Guid applicantId, Referrer referrer)
        {
            // Arrange
            var token = _onfidoTokenFixture.Create(applicantId.ToString(), referrer.Value);

            // Act
            Func<Task> result = async () => await _onfidoApiClient.SdkToken.GenerateAsync(token);

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }

        [Theory]
        public async Task ShouldDownloadDocument_WhenDocumentExist(NewApplicant applicant, DocumentContainer container)
        {
            // Arrange 
            await using var stream = new MemoryStream();
            await container.File.Data.CopyToAsync(stream);
            var expectedData = stream.ToArray();
            container.File.Data.Seek(0, SeekOrigin.Begin);

            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(onfidoApplicant.Id);
            var uploadedDocument = await _onfidoApiClient.Documents.UploadAsync(
                onfidoApplicant.Id,
                container.MetaData.OnfidoType,
                container.File);

            // Act
            var result = await _onfidoApiClient.Documents.DownloadAsync(uploadedDocument.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveSameCount(expectedData);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Theory(MaxTest = 3)]
        public async Task ShouldRunTwoChecksSimultaneously(NewApplicant applicant, DocumentContainer container, LivePhotoContainer livePhotoContainer)
        {
            // Given 
            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            var applicantId = onfidoApplicant.Id;
            applicantIds.Add(applicantId);
            var passport = await _onfidoApiClient.Documents.UploadAsync(applicantId, container.MetaData.OnfidoType, container.File);
            var selfie = await _onfidoApiClient.LivePhotos.UploadAsync(applicantId, livePhotoContainer.File);

            // Arrange
            var knownFace = new CheckRequest
            {
                ApplicantId = applicantId,
                ReportNames = new List<string> { "known_faces" },
                Asynchronous = true,
                ApplicantProvidesData = false
            };
            var facialSimilarity = new CheckRequest
            {
                ApplicantId = applicantId,
                DocumentIds = new List<string>() { passport.Id, selfie.Id },
                ReportNames = new List<string> { "facial_similarity_photo" },
                Asynchronous = true,
                ApplicantProvidesData = false
            };

            // Act
            Func<Task> act = () =>
            {
                var knownFaceCheckTask = _onfidoApiClient.Checks.CreateAsync(knownFace);
                var facialSimilarityCheckTask = _onfidoApiClient.Checks.CreateAsync(facialSimilarity);
                return Task.WhenAll(knownFaceCheckTask, facialSimilarityCheckTask);
            };

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Theory]
        public async Task ShouldThrowException_WhenDocumentNotExist(Guid documentId)
        {
            // Act
            Func<Task> result = async () => await _onfidoApiClient.Documents.DownloadAsync(documentId.ToString());

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }

        [Theory]
        public async Task ShouldDownloadSelfieFromLivePhotos_WhenLivePhotoExist(NewApplicant applicant,
                                                                                LivePhotoContainer container)
        {
            // Arrange 
            await using var stream = new MemoryStream();
            await container.File.Data.CopyToAsync(stream);
            var expectedData = stream.ToArray();
            container.File.Data.Seek(0, SeekOrigin.Begin);

            var onfidoApplicant = await _onfidoApiClient.Applicants.CreateAsync(applicant);
            applicantIds.Add(onfidoApplicant.Id);
            var uploadedPhoto = await _onfidoApiClient.LivePhotos.UploadAsync(
                onfidoApplicant.Id,
                container.File);

            // Act
            var result = await _onfidoApiClient.LivePhotos.DownloadAsync(uploadedPhoto.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveSameCount(expectedData);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Theory]
        public async Task ShouldThrowException_WhenLivePhotoNotExist(Guid livePhotoId)
        {
            // Act
            Func<Task> result = async () => await _onfidoApiClient.LivePhotos.DownloadAsync(livePhotoId.ToString());

            // Assert
            await result.Should().ThrowAsync<OnfidoApiErrorException>();
        }
    }
}
