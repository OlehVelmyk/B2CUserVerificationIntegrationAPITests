using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ICheckProviderService
    {
        Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId);

        Task RunAsync(CheckRunningContextDto[] contexts);

        Task ProcessAsync(CheckProcessingContextDto context);
    }
}
