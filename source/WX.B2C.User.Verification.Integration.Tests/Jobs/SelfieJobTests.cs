using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Serilog.Core;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Worker.Jobs.Clients;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.IoC;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs
{
    internal class SelfieJobTests : BaseIntegrationTest
    {
        private SelfieJob _sut;
        private JobDataMap _dataMap;
        private IJobExecutionContext _context;
        private IOnfidoApiClient _onfidoApiClient;
        private IExternalProfileRepository _externalProfileRepository;
        private IDocumentStorage _documentStorage;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterOnfidoClient();

            containerBuilder.Register(c => Substitute.For<IHostSettingsProvider>()).SingleInstance();
            containerBuilder.RegisterBlobStorage();
            containerBuilder.RegisterDbQueryFactory();
            containerBuilder.RegisterOnfidoThrottler();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _context = Substitute.For<IJobExecutionContext>();
            _dataMap = Substitute.For<JobDataMap>();
            _context.MergedJobDataMap.Returns(_ => _dataMap);

            var dataProvider = new ApplicantDataProvider(Resolve<IQueryFactory>(),Resolve<ICsvBlobStorage>());

            _sut = new SelfieJob(Resolve<IDocumentRepository>(),
                                 Resolve<IFileRepository>(),
                                 Resolve<IThrottledOnfidoApiClientFactory>(),
                                 dataProvider,
                                 Logger.None);

            _onfidoApiClient = Resolve<IOnfidoApiClientFactory>().Create();
            _externalProfileRepository = Resolve<IExternalProfileRepository>();
            _documentStorage = Resolve<IDocumentStorage>();

            Arb.Register<OnfidoApplicantArbitrary>();
            Arb.Register<LivePhotoContainerArbitrary>();
        }

        [Theory]
        public async Task ShouldSavePhotoSelfie(NewApplicant newApplicant, LivePhotoContainer livePhoto, Guid userId)
        {
            // Given
            var applicant = await _onfidoApiClient.Applicants.CreateAsync(newApplicant);
            var uploadedSelfie = await _onfidoApiClient.LivePhotos.UploadAsync(applicant.Id, livePhoto.File);

            var externalProfile = new ExternalProfileDto
            {
                Id = applicant.Id,
                Provider = ExternalProviderType.Onfido
            };
            await _externalProfileRepository.SaveAsync(userId, externalProfile);

            // Arrange
            var batchJobSettings = new SelfieJobSettings
            {
                Users = new[] { userId },
                ProcessBatchSize = 1,
                ReadingBatchSize = 1,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            // Act
            await _sut.Execute(_context);

            // Assert
            var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
            var selfieDocument = documents.Should().HaveCount(1).And.Subject.First();
            selfieDocument.Category.Should().Be(DocumentCategory.Selfie);
            selfieDocument.Type.Should().Be(SelfieDocumentType.Photo);
            var selfieFile = selfieDocument.Files.Should().HaveCount(1).And.Subject.First();
            selfieFile.FileName.Should().Be(uploadedSelfie.FileName);
            selfieFile.ExternalId.Should().Be(uploadedSelfie.Id);
        }

        [Theory]
        [Ignore("Only manual test")]
        public async Task ShouldSaveVideoSelfie(NewApplicant newApplicant, /*LiveVideoContainer liveVideo,*/ Guid userId)
        {
            // Given
            //var applicant = await _onfidoApiClient.Applicants.CreateAsync(newApplicant);
            //var uploadedSelfie = await _onfidoApiClient.LiveVideos.UploadAsync(Guid.Parse(applicant.Id), liveVideo.File);

            var applicantId = "3d3d4a2b-fa6d-4021-9c22-02be30af3e6c"; //applicant.Id;
            var externalProfile = new ExternalProfileDto
            {
                Id = applicantId,
                Provider = ExternalProviderType.Onfido
            };
            await _externalProfileRepository.SaveAsync(userId, externalProfile);

            // Arrange
            var batchJobSettings = new SelfieJobSettings
            {
                Users = new[] { userId },
                ProcessBatchSize = 1,
                ReadingBatchSize = 1,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            // Act
            await _sut.Execute(_context);

            // Assert
            var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
            var selfieDocument = documents.Should().HaveCount(1).And.Subject.First();
            selfieDocument.Category.Should().Be(DocumentCategory.Selfie);
            selfieDocument.Type.Should().Be(SelfieDocumentType.Video);
            selfieDocument.Files.Should().HaveCount(1);
        }

        [Theory]
        public async Task ShouldSaveLastPhotoSelfie(NewApplicant newApplicant,
                                                    LivePhotoContainer livePhoto1,
                                                    LivePhotoContainer livePhoto2,
                                                    Guid userId)
        {
            // Given
            var applicant = await _onfidoApiClient.Applicants.CreateAsync(newApplicant);
            var uploadedSelfie1 = await _onfidoApiClient.LivePhotos.UploadAsync(applicant.Id, livePhoto1.File);
            var uploadedSelfie2 = await _onfidoApiClient.LivePhotos.UploadAsync(applicant.Id, livePhoto2.File);

            var externalProfile = new ExternalProfileDto
            {
                Id = applicant.Id,
                Provider = ExternalProviderType.Onfido
            };
            await _externalProfileRepository.SaveAsync(userId, externalProfile);

            // Arrange
            var batchJobSettings = new SelfieJobSettings
            {
                Users = new[] { userId },
                ProcessBatchSize = 1,
                ReadingBatchSize = 1,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            // Act
            await _sut.Execute(_context);

            // Assert
            var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
            var selfieDocument = documents.Should().HaveCount(1).And.Subject.First();
            var selfieFile = selfieDocument.Files.Should().HaveCount(1).And.Subject.First();
            selfieFile.ExternalId.Should().Be(uploadedSelfie2.Id);
        }

        [Theory]
        public async Task ShouldIgnore_WhenNothingUploaded(NewApplicant newApplicant, Guid userId)
        {
            // Given
            var applicant = await _onfidoApiClient.Applicants.CreateAsync(newApplicant);

            var externalProfile = new ExternalProfileDto
            {
                Id = applicant.Id,
                Provider = ExternalProviderType.Onfido
            };
            await _externalProfileRepository.SaveAsync(userId, externalProfile);

            // Arrange
            var batchJobSettings = new SelfieJobSettings
            {
                Users = new[] { userId },
                ProcessBatchSize = 1,
                ReadingBatchSize = 1,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            // Act
            await _sut.Execute(_context);

            // Assert
            var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
            documents.Should().BeEmpty();
        }
    }
}