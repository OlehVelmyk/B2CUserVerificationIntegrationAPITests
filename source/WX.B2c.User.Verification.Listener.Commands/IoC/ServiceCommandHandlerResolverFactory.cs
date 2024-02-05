using System;
using WX.Commands.StorageQueue;

namespace WX.B2C.User.Verification.Listener.Commands.IoC
{
    internal class ServiceCommandHandlerResolverFactory : CommandHandlerResolverFactory
    {
        private readonly ICommandHandlerResolver _commandHandlerResolver;

        public ServiceCommandHandlerResolverFactory(ICommandHandlerResolver commandHandlerResolver)
        {
            _commandHandlerResolver = commandHandlerResolver ?? throw new ArgumentNullException(nameof(commandHandlerResolver));
        }

        public override ICommandHandlerResolver GetCommandHandlerResolver(string queueName) =>
            _commandHandlerResolver;
    }
}