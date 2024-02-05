using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface ICheckActor : IActor
    {
        Task RequestAsync(Guid userId, NewCheckDto newCheckDto, InitiationDto initiation);

        Task StartExecutionAsync(Guid checkId, CheckExecutionContextDto executionContextDto);

        Task FinishExecutionAsync(Guid checkId, CheckExecutionResultDto executionResultDto);

        Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResultDto processingResultDto);

        Task SaveErrorResultAsync(Guid checkId, CheckErrorResultDto errorResultDto);

        Task CancelAsync(Guid checkId);
    }
}