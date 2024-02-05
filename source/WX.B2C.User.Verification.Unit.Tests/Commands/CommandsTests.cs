using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Unit.Tests.Extensions;
using WX.Commands;

namespace WX.B2C.User.Verification.Unit.Tests.Commands
{
    [TestFixture]
    internal class CommandsTests
    {
        [Test]
        public void CommandsInDomain_ShouldBeInInheritedFromVerificationCommand()
        {
            var assembly = typeof(SubmitCollectionStepCommand).Assembly;
            var commands = assembly.GetTypes().Where(type => typeof(Command).IsAssignableFrom(type));

            commands.Should().AllTypesBeAssignableTo<VerificationCommand>();
        }

        [Test]
        public void DomainCommands_ShouldBeInTestSource()
        {
            var baseType = typeof(VerificationCommand);
            var assembly = baseType.Assembly;
            var expected = assembly.GetTypes().Where(type => baseType.IsAssignableFrom(type) && type != baseType);
            var actual = DomainCommandsSource().Select(command => command.GetType());

            expected.Except(actual).Should().BeEmpty();
        }

        [Test]
        [TestCaseSource(nameof(DomainCommandsSource))]
        public void Command_ShouldHaveNotEmptyChainIdAndId(VerificationCommand command)
        {
            command.CommandId.Should().NotBeEmpty();
            command.CommandChainId.Should().NotBeEmpty();
        }

        private static IEnumerable<VerificationCommand> DomainCommandsSource()
        {
            var reason = Guid.NewGuid().ToString();
            var xPath = Guid.NewGuid().ToString();
            var taskType = Guid.NewGuid().ToString();
            var checkType = Guid.NewGuid().ToString();
            var checkProvider = Guid.NewGuid().ToString();
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

            yield return new AddRequiredTasksCommand(Guid.NewGuid(), Guid.NewGuid(), ids, reason);

            yield return new ApproveApplicationCommand(Guid.NewGuid(), Guid.NewGuid(), true, reason);

            yield return new CreateCollectionStepCommand(Guid.NewGuid(), Guid.NewGuid(), xPath, true, true, true, reason);

            yield return new CreateTaskCommand(Guid.NewGuid(), Guid.NewGuid(), taskType, Guid.NewGuid(), ids, ids, reason);

            yield return new IncompleteTaskCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            yield return new MoveApplicationInReviewCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            yield return new PassTaskCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            yield return new RejectApplicationCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            yield return new SubmitCollectionStepCommand(Guid.NewGuid(), reason);

            yield return new RequestCheckCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), checkType, checkProvider, ids, reason);
			
            yield return new AutomateApplicationCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            yield return new CompleteTriggerCommand(Guid.NewGuid());

            yield return new CreateExternalProfileCommand(Guid.NewGuid(), checkProvider);
        }
    }
}