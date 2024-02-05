using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class FileActorClient : IFileService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ProfileActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public FileActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task<Guid> UploadAsync(Guid userId, UploadedFileDto uploadedFileDto) =>
            CreateActorProxy(userId).Execute(actor => actor.UploadFileAsync(userId, uploadedFileDto));

        public Task UpdateAsync(Guid userId, Guid fileId, ExternalFileData fileData) => 
            CreateActorProxy(userId).Execute(actor => actor.UpdateFileAsync(userId, fileId, fileData));


        public Task SubmitAsync(Guid userId, Guid fileId) =>
            CreateActorProxy(userId).Execute(actor => actor.SubmitFileAsync(userId, fileId));

        private IServiceClientProxy<IProfileActor> CreateActorProxy(Guid actorId) =>
            _serviceClientFactory.CreateActorProxy<IProfileActor>(new ActorId(actorId), ServiceUri);
    }
}