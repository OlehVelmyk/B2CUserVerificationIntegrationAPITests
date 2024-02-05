using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface ICollectionStepManager
    {
        Task TrySubmitAsync(IReadOnlyCollection<CollectionStepDto> collectionSteps, string reason);
    }

    internal class CollectionStepManager : ICollectionStepManager
    {
        private readonly IBatchCommandPublisher _commandsPublisher;

        public CollectionStepManager(IBatchCommandPublisher commandsPublisher)
        {
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
        }

        public async Task TrySubmitAsync(IReadOnlyCollection<CollectionStepDto> collectionSteps, string reason)
        {
            if (collectionSteps == null)
                throw new ArgumentNullException(nameof(collectionSteps));
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            var commands = collectionSteps
                           .Select(step => new SubmitCollectionStepCommand(step.Id, reason))
                           .ToArray();

            await _commandsPublisher.PublishAsync(commands);
        }
    }
}
