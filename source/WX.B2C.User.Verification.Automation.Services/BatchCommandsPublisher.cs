using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.Commands;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface IBatchCommandPublisher
    {
        Task PublishAsync(Command command, CancellationToken cancellationToken = default);

        Task PublishAsync(IEnumerable<Command> commands, CancellationToken cancellationToken = default);
    }

    internal class BatchCommandPublisher : IBatchCommandPublisher
    {
        private readonly ICommandsPublisher _commandsPublisher;

        public BatchCommandPublisher(ICommandsPublisher commandsPublisher)
        {
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
        }

        public Task PublishAsync(Command command, CancellationToken cancellationToken = default) =>
            PublishAsync(new[] { command }, cancellationToken);

        public async Task PublishAsync(IEnumerable<Command> commands, CancellationToken cancellationToken = default)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            foreach (var command in commands)
                await _commandsPublisher.PublishAsync(command, cancellationToken);
        }
    }
}
