using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Facade.Commands.Middleware;
using WX.Commands;

namespace WX.B2C.User.Verification.Listener.Commands
{
    internal class CommandHandlerWrapper<T> : ICommandHandler<T> where T : Command
    {
        private readonly Func<T, Task> _handlingFunc;

        public CommandHandlerWrapper(IEnumerable<ICommandHandlerDecorator> commandHandlerDecorators,
                                     ICommandHandler handler)
        {
            _handlingFunc = ((ICommandHandler<T>)handler).HandleAsync;
            foreach (var commandHandlerDecorator in commandHandlerDecorators)
            {
                var innerFunc = _handlingFunc;
                _handlingFunc = command => commandHandlerDecorator.HandleAsync(command, innerFunc);
            }
        }

        public Task HandleAsync(T command) =>
            _handlingFunc(command);
    }
}