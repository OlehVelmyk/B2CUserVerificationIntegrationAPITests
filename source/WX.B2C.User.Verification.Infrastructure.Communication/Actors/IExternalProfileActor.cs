using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface IExternalProfileActor : IActor
    {
        Task<ExternalProfileDto> GetOrCreateAsync(Guid userId, ExternalProviderType externalProviderType);
    }
}