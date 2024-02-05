using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class CheckCommandHandler : ICommandHandler<RequestCheckCommand>
    {
        private readonly ICheckService _checkService;

        public CheckCommandHandler(ICheckService checkService)
        {
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        }

        public Task HandleAsync(RequestCheckCommand command)
        {
            return _checkService.RequestAsync(command.UserId,
                new NewCheckDto
                {
                    Id = command.CommandId,
                    VariantId = command.VariantId,
                    CheckType = command.Type.To<CheckType>(),
                    Provider = command.Provider.To<CheckProviderType>(),
                    RelatedTasks = command.RelatedTasks
                }, InitiationDto.CreateSystem(command.Reason));
        }
    }
}
