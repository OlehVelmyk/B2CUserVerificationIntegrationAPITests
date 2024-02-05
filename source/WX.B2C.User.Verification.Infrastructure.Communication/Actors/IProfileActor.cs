using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface IProfileActor : IActor
    {
        public Task UpdatePersonalDetailsAsync(Guid userId, PersonalDetailsPatch patch, InitiationDto initiationDto);

        public Task UpdateVerificationDetailsAsync(Guid userId, VerificationDetailsPatch patch, InitiationDto initiationDto);

        Task UpdateExternalProfileAsync(Guid userId, ExternalProfileDto externalProfile);

        Task<Guid> UploadFileAsync(Guid userId, UploadedFileDto uploadFileDto);

        Task UpdateFileAsync(Guid userId, Guid fileId, ExternalFileData fileData);

        Task SubmitFileAsync(Guid userId, Guid fileId);

        Task SubmitDocumentAsync(Guid userId, SubmitDocumentDto submitDocumentDto, InitiationDto initiationDto);

        Task ArchiveDocumentAsync(Guid userId, Guid documentId, InitiationDto initiationDto);
    }
}