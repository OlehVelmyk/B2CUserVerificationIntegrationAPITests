using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Services
{
    public interface ICheckProviderStatelessService : IService
    {
        Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId);

        Task RunAsync(CheckRunningContextDto[] contexts);

        Task ProcessAsync(CheckProcessingContextDto context);
    }
}
