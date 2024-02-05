using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class CollectionStepCommandHandler : ICommandHandler<CreateCollectionStepCommand>, 
                                                  ICommandHandler<SubmitCollectionStepCommand>
    {
        private readonly ICollectionStepService _collectionStepService;

        public CollectionStepCommandHandler(ICollectionStepService collectionStepService)
        {
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
        }

        public async Task HandleAsync(CreateCollectionStepCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var newStep = new NewCollectionStepDto
            {
                Id = command.CommandId,
                XPath = command.XPath, 
                IsRequired = command.IsRequired, 
                IsReviewNeeded = command.IsReviewNeeded
            };
            var initiation = InitiationDto.CreateSystem(command.Reason);
            var stepId = await _collectionStepService.RequestAsync(command.UserId, newStep, initiation);

            if (command.IsSubmitted)
                await _collectionStepService.SubmitAsync(stepId, initiation);
        }

        public Task HandleAsync(SubmitCollectionStepCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var initiation = InitiationDto.CreateSystem(command.Reason);
            return _collectionStepService.SubmitAsync(command.StepId, initiation);
        }
    }
}
