using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Actor.Actors
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    [ActorService(Name = nameof(ProfileActor))]
    internal class ProfileActor : Microsoft.ServiceFabric.Actors.Runtime.Actor, IProfileActor
    {
        private readonly IProfileService _profileService;
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ProfileActor(
            ActorService actorService, 
            ActorId actorId, 
            IProfileService profileService, 
            IDocumentService documentService, 
            IFileService fileService)
            : base(actorService, actorId)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }

        public Task UpdatePersonalDetailsAsync(Guid userId, PersonalDetailsPatch patch, InitiationDto initiationDto) =>
            _profileService.UpdateAsync(userId, patch, initiationDto);

        public Task UpdateVerificationDetailsAsync(Guid userId, VerificationDetailsPatch patch, InitiationDto initiationDto) =>
            _profileService.UpdateAsync(userId, patch, initiationDto);

        public Task<Guid> UploadFileAsync(Guid userId, UploadedFileDto uploadFileDto) =>
            _fileService.UploadAsync(userId, uploadFileDto);

        public Task UpdateFileAsync(Guid userId, Guid fileId, ExternalFileData fileData) => 
            _fileService.UpdateAsync(userId, fileId, fileData);

        public Task SubmitFileAsync(Guid userId, Guid fileId) => 
            _fileService.SubmitAsync(userId, fileId);

        public Task SubmitDocumentAsync(Guid userId, SubmitDocumentDto submitDocumentDto, InitiationDto initiationDto) =>
            _documentService.SubmitAsync(userId, submitDocumentDto, initiationDto);

        public Task ArchiveDocumentAsync(Guid userId, Guid documentId, InitiationDto initiationDto) =>
            _documentService.ArchiveAsync(userId, documentId, initiationDto);

        public Task UpdateExternalProfileAsync(Guid userId, ExternalProfileDto externalProfile) => 
            _profileService.UpdateAsync(userId, externalProfile);
    }
}