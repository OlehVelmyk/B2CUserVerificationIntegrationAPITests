using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Infrastructure.Remoting;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands.Middleware
{
    internal class OperationContextDecorator : ICommandHandlerDecorator
    {
        private readonly IOperationContextScopeFactory _contextScopeFactory;

        public OperationContextDecorator(IOperationContextScopeFactory contextScopeFactory)
        {
            _contextScopeFactory = contextScopeFactory ?? throw new ArgumentNullException(nameof(contextScopeFactory));
        }

        public async Task HandleAsync<T>(T command, Func<T, Task> processor) where T : Command
        {
            var correlationId = Guid.NewGuid();
            var causationId = Guid.NewGuid();
            if (command is IOperation operation)
            {
                correlationId = operation.CorrelationId;
                causationId = operation.OperationId;
            }

            var operationName = $"Handle {command.GetType().Name}";
            using var context = _contextScopeFactory.Create(correlationId, causationId, operationName);
            await processor(command);
        }
    }
}