using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Serilog;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Facade.Commands.IoC;
using WX.B2C.User.Verification.Infrastructure.Remoting;
using WX.Commands;
using WX.Commands.StorageQueue;

using CommandHandlerResolver = WX.B2C.User.Verification.Listener.Commands.CommandHandlerResolver;

namespace WX.B2C.User.Verification.Unit.Tests.Commands
{
    [Explicit]
    [TestFixture]
    internal class CommandHandlerRegistrationTests
    {
        private ICommandHandlerResolver _resolver;
        private IOperationContextScopeFactory _operationContextFactory;
        private TestLogger _logger;

        [SetUp]
        public void Setup()
        {
            _operationContextFactory = Substitute.For<IOperationContextScopeFactory>();
            _logger = new TestLogger();

            var builder = new ContainerBuilder();
            
            builder.Register(_ => _operationContextFactory).As<IOperationContextScopeFactory>();
            builder.Register(_ => _logger).As<ILogger>();

            builder.RegisterType<CommandHandlerResolver>().As<ICommandHandlerResolver>();
            builder.Register<TestCommandHandler>();
            builder.RegisterMiddleware();

            var container = builder.Build();

            _resolver = container.Resolve<ICommandHandlerResolver>();
        }

        [Theory]
        public async Task ShouldSetCorrelationId(Guid correlationId, Guid operationId)
        {
            var command = new TestCommand()
            {
                CorrelationId = correlationId,
                OperationId = operationId
            };
            
            var handler = ResolveHandler(command);
            await handler(command);

            _operationContextFactory.Received().Create(correlationId, operationId, Arg.Any<string>());
        }

        [Test]
        public async Task ShouldLogError()
        {
            var command = new ThrowExceptionCommand();
            var handler = ResolveHandler(command);

            try
            {
                await handler(command);
            }
            catch
            {
                // ignored
            }

            _logger.HasErrors.Should().BeTrue();
        }

        /// <summary>
        /// Resolve handler like in WX.Commands.StorageQueue.
        /// </summary>
        private CommandHandler ResolveHandler<T>(T command) where T : Command
        {
            var handlers = _resolver.GetHandlers();
            var handlerModel = handlers.Single(model => model.CommandType == command.GetType());
            var commandHandlerConstructor = new CommandHandlerConstructor();
            return commandHandlerConstructor.ConstructSingle(handlerModel);
        }

        private class TestCommandHandler :
            ICommandHandler<TestCommand>,
            ICommandHandler<ThrowExceptionCommand>
        {
            public Task HandleAsync(TestCommand command)
            {
                Console.WriteLine($"{command.GetType()}");
                return Task.CompletedTask;
            }

            public Task HandleAsync(ThrowExceptionCommand command)
            {
                Console.WriteLine($"{command.GetType()}");
                throw new Exception("Command not valid");
            }
        }

        class TestCommand : VerificationCommand
        {

        }

        class ThrowExceptionCommand : VerificationCommand
        {

        }
    }
}