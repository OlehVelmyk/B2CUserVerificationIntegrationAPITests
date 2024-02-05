using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.PassFort;
using WX.B2C.User.Verification.PassFort.Models;

namespace WX.B2C.User.Verification.Listener.PassFort.Services
{
    public interface IPassFortSynchronizationService
    {
        Task UpdateNewProfileAsync(Guid userId, string profileId);

        Task CreateNewApplicationAsync(string profileId, string productId, Guid applicationId);

        Task UpdateApplicationStateAsync(string profileId, string productId, ApplicationState previousState, ApplicationState newState);

        Task UpdateTaskStateAsync(string profileId, TaskType taskType, TaskResult? taskResult);

        Task UploadDocumentAsync(string profileId, Guid documentId);

        Task UpdatePersonalDetailsAsync(string profileId, PropertyChangeDto[] changes);

        Task UpdateVerificationDetailsAsync(Guid userId, string profileId, string[] changes);
    }

    internal class PassFortSynchronizationService : IPassFortSynchronizationService
    {
        private readonly TaskType[] _allowedTaskTypes =
        {
            TaskType.Address,
            TaskType.Identity,
            TaskType.RiskListsScreening
        };
        private readonly string[] _allowedDocumentTypes =
        {
            IdentityDocumentType.Passport,
            IdentityDocumentType.IdentityCard,
            IdentityDocumentType.DriverLicense,
            IdentityDocumentType.BirthCertificate,
            IdentityDocumentType.Other,

            AddressDocumentType.CertificateOfResidency,
            AddressDocumentType.BankStatement,
            AddressDocumentType.UtilityBill,
            AddressDocumentType.TaxReturn,
            AddressDocumentType.CouncilTax
        };

        private readonly IProfileStorage _profileStorage;
        private readonly ITaskStorage _taskStorage;
        private readonly IPassFortGateway _gateway;
        private readonly IDocumentStorage _documentStorage;
        private readonly IFileProvider _fileProvider;
        private readonly IPassFortProfileUpdater _profileUpdater;
        private readonly ILogger _logger;

        public PassFortSynchronizationService(IProfileStorage profileStorage,
                                              ITaskStorage taskStorage,
                                              IPassFortGateway gateway,
                                              IDocumentStorage documentStorage,
                                              IFileProvider fileProvider,
                                              IPassFortProfileUpdater profileUpdater,
                                              ILogger logger)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _profileUpdater = profileUpdater ?? throw new ArgumentNullException(nameof(profileUpdater));
            _logger = logger?.ForContext<PassFortSynchronizationService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateNewProfileAsync(Guid userId, string profileId)
        {
            var tasks = await _taskStorage.GetAllAsync(userId);
            var completedTasks = tasks.Where(task => task.State == TaskState.Completed);
            foreach (var task in completedTasks)
                await UpdateTaskStateAsync(profileId, task.Type, task.Result);

            var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
            foreach (var document in documents)
                await UploadDocument(profileId, document);
        }

        public async Task CreateNewApplicationAsync(string profileId, string productId, Guid applicationId)
        {
            var application = await _gateway.FindApplicationAsync(profileId, productId);
            if (application != null)
                return;

            await _gateway.CreateApplicationAsync(profileId, productId);
        }

        public async Task UpdateApplicationStateAsync(string profileId, string productId, ApplicationState previousState, ApplicationState newState)
        {
            var passfortApplication = await _gateway.GetApplicationAsync(profileId, productId);
            if (passfortApplication.State == newState)
            {
                _logger.Information("Application state sync for {ProfileId} skipped as state already {State}.", profileId, newState);
                return;
            }
            if (newState == ApplicationState.Approved && passfortApplication.IsApproveBlocked)
            {
                _logger.Error("Application for {ProfileId} is approved, but PassFort has approval blockers.", profileId);
                //TODO PHASE 2 Would be nice to notify compliance team
                return;
            }

            if (ShouldRevert())
                await _gateway.RevertApplicationDecisionAsync(profileId, passfortApplication.Id);
            else
                await _gateway.UpdateApplicationStateAsync(profileId, passfortApplication.Id, newState);

            bool ShouldRevert() => previousState is ApplicationState.Cancelled or ApplicationState.Rejected;
        }

        public async Task UpdateTaskStateAsync(string profileId, TaskType taskType, TaskResult? taskResult)
        {
            if (ShouldNotCreate()) return;

            var passFortTask = await _gateway.GetTaskAsync(profileId, taskType)
                               ?? await _gateway.CreateTaskAsync(profileId, taskType);

            if (passFortTask.IsExpired.GetValueOrDefault())
                return;

            await _gateway.UpdateTaskStateAsync(profileId, passFortTask.Id, taskResult);

            bool ShouldNotCreate() => !_allowedTaskTypes.Contains(taskType);
        }

        public async Task UploadDocumentAsync(string profileId, Guid documentId)
        {
            var documentDto = await _documentStorage.GetAsync(documentId);
            await UploadDocument(profileId, documentDto);
        }

        public async Task UpdatePersonalDetailsAsync(string profileId, PropertyChangeDto[] changes)
        {
            var emailChange = changes.Find<string>(XPathes.Email);
            if (emailChange is null)
                return;

            var profilePatch = new PassFortProfilePatch { Email = emailChange.NewValue.Some() };
            await _profileUpdater.UpdateAsync(profileId, profilePatch);
        }

        public async Task UpdateVerificationDetailsAsync(Guid userId, string profileId, string[] changes)
        {
            if (!changes.ContainsAny(XPathes.PoiIssuingCountry, XPathes.IdDocumentNumber))
                return;

            var verificationDetails = await _profileStorage.FindVerificationDetailsAsync(userId);
            var idDocumentData = (verificationDetails.PoiIssuingCountry, verificationDetails.IdDocumentNumber);
            var profilePatch = new PassFortProfilePatch { IdDocumentData = idDocumentData.Some() };
            await _profileUpdater.UpdateAsync(profileId, profilePatch);
        }

        private async Task UploadDocument(string profileId, DocumentDto document)
        {
            if (ShouldNotUpload(document.Type))
                return;

            var imageIds = new List<string>();
            foreach (var file in document.Files)
            {
                var fileStream = await _fileProvider.DownloadAsync(document.UserId, file);
                var imageId = await _gateway.UploadDocumentImageAsync(profileId, fileStream);
                imageIds.Add(imageId);
            }

            await _gateway.UploadDocumentAsync(profileId, document.Category, document.Type, imageIds);

            bool ShouldNotUpload(string type) => !_allowedDocumentTypes.Contains(type);
        }
    }
}