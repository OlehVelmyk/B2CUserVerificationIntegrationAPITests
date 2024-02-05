using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.Commands;

namespace WX.B2C.User.Verification.Listener.Events.Middleware
{
    internal class CommandOperationContextDecorator : ICommandsPublisher
    {
        private readonly ICommandsPublisher _commandsPublisher;
        private readonly IOperationContextProvider _operationContextProvider;

        public CommandOperationContextDecorator(ICommandsPublisher commandsPublisher,
                                                IOperationContextProvider operationContextProvider)
        {
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
        }

        public Task PublishAsync<TCommand>(TCommand command, CancellationToken cancellationToken, TimeSpan? visibilityTimeout = null) where TCommand : Command
        {
            if (command is VerificationCommand verificationCommand)
            {
                var context = _operationContextProvider.GetContextOrDefault();
                verificationCommand.CorrelationId = context.CorrelationId;
                verificationCommand.OperationId = context.OperationId;
            }

            return _commandsPublisher.PublishAsync(command, cancellationToken, visibilityTimeout);
        }
    }
}