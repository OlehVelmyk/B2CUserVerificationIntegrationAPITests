using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Facade.Commands.Middleware;
using WX.Commands;
using WX.Commands.StorageQueue;

namespace WX.B2C.User.Verification.Listener.Commands
{
    internal class CommandHandlerResolver : ICommandHandlerResolver
    {
        private readonly HandlerModel[] _handlers;

        public CommandHandlerResolver(IEnumerable<ICommandHandler> commandHandlers, IEnumerable<ICommandHandlerDecorator> commandHandlerDecorators)
        {
            var handlerDecorators = commandHandlerDecorators.ToArray();
            var result = new List<HandlerModel>();
            var handlersWithEvents = commandHandlers.ToDictionary(x => x, x => ExploreHandlers(x.GetType()));

            foreach (var (handler, types) in handlersWithEvents)
            {
                result.AddRange(types.Select(type =>
                {
                    var decorator = Wrap(handler, type, handlerDecorators);
                    var handlerModel = new HandlerModel(decorator, type);
                    return handlerModel;
                }));
            }

            _handlers = result.ToArray();
        }

        public HandlerModel[] GetHandlers()
        {
            return _handlers;
        }

        private static IEnumerable<Type> ExploreHandlers(Type implementation) =>
            implementation.GetInterfaces().
                           Where(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                           .Select(x =>
                           {
                               var command = x.GetGenericArguments().First();
                               return command;
                           });

        private static ICommandHandler Wrap(ICommandHandler handler,
                                            Type commandType,
                                            ICommandHandlerDecorator[] decorators)
        {
            if (decorators.Length == 0)
                return handler;

            var ctor = typeof(CommandHandlerWrapper<>).MakeGenericType(commandType).GetConstructor(new[] { typeof(IEnumerable<ICommandHandlerDecorator>), typeof(ICommandHandler) });
            var wrapper = ctor.Invoke(new object[] { decorators, handler });
            return (ICommandHandler)wrapper;
        }
    }
}