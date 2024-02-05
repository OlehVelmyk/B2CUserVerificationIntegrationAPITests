using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Commands
{
    public interface ICommandService
    {
        Task RunAsync(AddTaskCommand command, InitiationDto initiationDto);

        Task RunAsync(AddCollectionStepsToTaskCommand command, InitiationDto initiationDto);

        Task<Guid> RunAsync(AddCollectionStepCommand command, InitiationDto initiationDto);

        Task RunAsync(InstructCheckCommand command, InitiationDto initiationDto);
    }
}