using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class ApplicationCommandHandler : ICommandHandler<AddRequiredTasksCommand>,
                                               ICommandHandler<RejectApplicationCommand>,
                                               ICommandHandler<ApproveApplicationCommand>,
                                               ICommandHandler<MoveApplicationInReviewCommand>,
                                               ICommandHandler<AutomateApplicationCommand>
    {
        private readonly IApplicationService _applicationService;
        private readonly ITicketService _ticketService;

        public ApplicationCommandHandler(IApplicationService applicationService, ITicketService ticketService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        }

        public Task HandleAsync(AddRequiredTasksCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiationDto = InitiationDto.CreateSystem(command.Reason);
            return _applicationService.AddRequiredTasksAsync(command.ApplicationId, command.TaskIds, initiationDto);
        }

        public Task HandleAsync(ApproveApplicationCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.ApproveManually)
                return SendReviewTicketAsync(command.UserId, command.ApplicationId);

            var initiationDto = InitiationDto.CreateSystem(command.Reason);
            return _applicationService.ApproveAsync(command.ApplicationId, initiationDto);
        }

        public Task HandleAsync(MoveApplicationInReviewCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiationDto = InitiationDto.CreateSystem(command.Reason);
            return _applicationService.RequestReviewAsync(command.ApplicationId, initiationDto);
        }

        public Task HandleAsync(RejectApplicationCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiationDto = InitiationDto.CreateSystem(command.Reason);
            return _applicationService.RejectAsync(command.ApplicationId, initiationDto);
        }

        public Task HandleAsync(AutomateApplicationCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiationDto = InitiationDto.CreateSystem(command.Reason);
            return _applicationService.AutomateAsync(command.ApplicationId, initiationDto);
        }

        private Task SendReviewTicketAsync(Guid userId, Guid applicationId)
        {
            var ticketContext = new ApplicationTicketContext
            {
                UserId = userId,
                ApplicationId = applicationId,
                Reason = ApplicationTicketReason.ReadyForReview
            };
            return _ticketService.SendAsync(ticketContext);
        }
    }
}
