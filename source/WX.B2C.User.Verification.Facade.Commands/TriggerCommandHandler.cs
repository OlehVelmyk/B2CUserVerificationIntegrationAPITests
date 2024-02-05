using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class TriggerCommandHandler : ICommandHandler<CompleteTriggerCommand>
    {
        private readonly ITriggerService _triggerService;

        public TriggerCommandHandler(ITriggerService triggerService)
        {
            _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
        }

        public Task HandleAsync(CompleteTriggerCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _triggerService.CompleteAsync(command.TriggerId);
        }
    }
}
