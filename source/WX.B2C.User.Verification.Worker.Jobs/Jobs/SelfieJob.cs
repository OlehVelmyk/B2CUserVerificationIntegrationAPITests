using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Onfido;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Worker.Jobs.Clients;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using Tasks = WX.B2C.User.Verification.Extensions.TaskExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class SelfieJob : BatchJob<ApplicantData, SelfieJobSettings>
    {
        private const string NoPhotoOrVideoFound = "No photo and no video are found for user";
        private readonly IDocumentRepository _documentRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IThrottledOnfidoApiClientFactory _onfidoApiClientFactory;

        public SelfieJob(IDocumentRepository documentRepository,
                         IFileRepository fileRepository,
                         IThrottledOnfidoApiClientFactory onfidoApiClientFactory,
                         IApplicantDataProvider jobDataProvider,
                         ILogger logger)
            : base(jobDataProvider, logger)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
            _onfidoApiClientFactory = onfidoApiClientFactory ?? throw new ArgumentNullException(nameof(onfidoApiClientFactory));
        }

        public static string Name => "onfido-selfie";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<SelfieJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Import selfie (photo and video) from Onfido.");

        protected override async Task Execute(Batch<ApplicantData> batch, SelfieJobSettings settings, CancellationToken cancellationToken)
        {
            var onfidoClient = _onfidoApiClientFactory.Create(settings.OnfidoRequestPerMinute);
            foreach (var applicant in batch.Items)
            {
                var logger = Logger.ForContext(nameof(applicant.UserId), applicant.UserId);

                if (string.IsNullOrEmpty(applicant.Id))
                {
                    logger.Warning(NoPhotoOrVideoFound);
                    continue;
                }

                var selfies = await GetSelfieDataAsync(applicant, onfidoClient, logger);
                foreach (var selfie in selfies)
                {
                    await _fileRepository.SaveAsync(selfie.File);
                    await _documentRepository.SaveAsync(selfie.Document);
                    logger.Information("Saved selfie document {Type}. ExternalId: {ExternalId}. Id: {FileId}",
                                       selfie.Document.Type,
                                       selfie.File.ExternalId,
                                       selfie.File.Id);
                }
            }
        }

        private async Task<IEnumerable<Selfie>> GetSelfieDataAsync(ApplicantData applicant,
                                                                   IOnfidoApiClientWithThrottling client,
                                                                   ILogger logger)
        {
            try
            {
                LivePhotoList photos;
                LiveVideoList videos;

                var allPhotosTask = client.ListLivePhotosAsync(applicant.Id);
                var allVideosTask = client.ListLiveVideosAsync(applicant.Id);

                try
                {
                    (photos, videos) = await Tasks.WhenAll(allPhotosTask, allVideosTask);
                }
                catch (Exception e)
                {
                    logger.Error(e, NoPhotoOrVideoFound);
                    return Array.Empty<Selfie>();
                }

                if (!photos.LivePhotos.Any() && !videos.LiveVideos.Any())
                {
                    logger.Warning(NoPhotoOrVideoFound);
                    return Array.Empty<Selfie>();
                }

                return GetSelfieDataAsync(applicant.UserId, photos, videos);
            }
            catch (OnfidoApiErrorException exc)
            {
                throw new OnfidoApiException(exc.Message);
            }
        }

        private IEnumerable<Selfie> GetSelfieDataAsync(Guid userId,
                                                       LivePhotoList photos,
                                                       LiveVideoList videos)
        {
            if (photos.LivePhotos.Any())
            {
                var photo = photos.LivePhotos.OrderByDescending(photo => photo.CreatedAt).First();
                var documentType = SelfieDocumentType.Photo;
                var fileName = photo.FileName;
                var externalId = photo.Id;
                var createdAt = photo.CreatedAt ?? DateTime.UtcNow;
                var file = CreateFile(userId, fileName, externalId);
                var document = CreateDocument(userId, documentType, createdAt, file.Id);
                yield return new Selfie(document, file);
            }

            if (videos.LiveVideos.Any())
            {
                var video = videos.LiveVideos.OrderByDescending(video => video.CreatedAt).First();
                var documentType = SelfieDocumentType.Video;
                var fileName = video.FileName;
                var externalId = video.Id;
                var createdAt = video.CreatedAt ?? DateTime.UtcNow;
                var file = CreateFile(userId, fileName, externalId);
                var document = CreateDocument(userId, documentType, createdAt, file.Id);
                yield return new Selfie(document, file);
            }
        }

        private FileDto CreateFile(Guid userId, string fileName, string externalId) =>
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = fileName,
                Status = FileStatus.Submitted,
                Provider = ExternalFileProviderType.Onfido,
                ExternalId = externalId
            };

        private DocumentDto CreateDocument(Guid userId,
                                           string documentType,
                                           DateTime createdAt,
                                           Guid fileId)
        {
            var documentId = Guid.NewGuid();
            return new DocumentDto
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = DocumentCategory.Selfie,
                Type = documentType,
                Status = DocumentStatus.Submitted,
                CreatedAt = createdAt,
                Files = new[]
                {
                    new DocumentFileDto
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = documentId,
                        FileId = fileId
                    }
                }
            };
        }

        private class Selfie
        {
            public Selfie(DocumentDto document, FileDto file)
            {
                Document = document ?? throw new ArgumentNullException(nameof(document));
                File = file ?? throw new ArgumentNullException(nameof(file));
            }

            public DocumentDto Document { get; }

            public FileDto File { get; }
        }
    }
}