using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class TaskCommandHandler : ICommandHandler<CreateTaskCommand>,
                                        ICommandHandler<PassTaskCommand>,
                                        ICommandHandler<IncompleteTaskCommand>
    {
        private readonly ITaskService _taskService;

        public TaskCommandHandler(ITaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public Task HandleAsync(CreateTaskCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var newTask = new NewTaskDto
            {
                Id = command.CommandId,
                UserId = command.UserId, 
                VariantId = command.VariantId, 
                Type = command.Type.To<TaskType>(),
                CollectionStepIds = command.CollectionStepIds, 
                AcceptanceCheckIds = command.AcceptanceCheckIds
            };
            var initiation = InitiationDto.CreateSystem(command.Reason);
            return _taskService.CreateAsync(newTask, initiation);
        }

        public Task HandleAsync(PassTaskCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiation = InitiationDto.CreateSystem(command.Reason);
            return _taskService.CompleteAsync(command.Id, TaskResult.Passed, initiation);
        }

        public Task HandleAsync(IncompleteTaskCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiation = InitiationDto.CreateSystem(command.Reason);
            return _taskService.IncompleteAsync(command.Id, initiation);
        }
    }
}
