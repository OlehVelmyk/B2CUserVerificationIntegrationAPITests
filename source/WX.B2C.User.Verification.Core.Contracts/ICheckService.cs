using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ICheckService
    {
        Task RequestAsync(Guid userId, NewCheckDto newCheckDto, InitiationDto initiation);

        Task StartExecutionAsync(Guid checkId, CheckExecutionContextDto executionContextDto);

        Task FinishExecutionAsync(Guid checkId, CheckExecutionResultDto executionResultDto);

        Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResultDto processingResult);

        Task SaveErrorResultAsync(Guid checkId, CheckErrorResultDto errorResult);

        Task CancelAsync(Guid checkId);
    }
}
