using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Services.Tickets;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using TicketInfo = WX.B2C.User.Verification.Unit.Tests.Arbitraries.TicketInfo;

namespace WX.B2C.User.Verification.Unit.Tests.TicketProviders
{
    internal class TicketFactoryTests
    {
        private IReadOnlyDictionary<string, object> _existingParameters;
        private TicketFactory _ticketProvider;
        private TicketConfig _ticketConfig;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Arb.Register<TicketInfoArbitrary>();
        }

        [SetUp]
        public void Setup()
        {
            var ticketParametersReader = Substitute.For<ITicketParametersReader>();
            ticketParametersReader.ReadParametersAsync(Arg.Any<Guid>(), Arg.Any<string[]>(), Arg.Any<IReadOnlyDictionary<string, object>>())
                                  .ReturnsForAnyArgs(_ => Task.FromResult(_existingParameters));

            var optionProvider = Substitute.For<IOptionProvider<TicketsOption>>();
            optionProvider.GetAsync().Returns(info => new TicketsOption(new[] { _ticketConfig }));
            _ticketProvider = new TicketFactory(ticketParametersReader, optionProvider);
        }

        [Theory]
        public async Task ShouldCreateTicket(TicketInfo ticketInfo, Guid userId)
        {
            var reason = "test template";
            _ticketConfig = new TicketConfig(reason,
                                             ticketInfo.Parameters.Keys.ToArray(),
                                             new Dictionary<string, string>());
            _existingParameters = ticketInfo.Parameters;
            var result = await _ticketProvider.CreateAsync(reason, userId, null);

            result.Should().NotBeNull();
            result.Reason.Should().Be(reason);
            result.Parameters.Should().NotBeNullOrEmpty();
            using var _ = new AssertionScope();
            foreach (var parameter in _existingParameters)
            {
                result.Parameters.Should().ContainKey(parameter.Key).WhoseValue.Should().Be(parameter.Value.ToString());
            }
        }

        [Theory]
        public async Task ShouldFormatValues_WhenCreateTicketWithFormats(TicketInfo ticketInfo, Guid userId)
        {
            var reason = "test template";
            var formats = new Dictionary<string, string>
            {
                { ticketInfo.DateTimeXpath, "D" },
                { ticketInfo.DateTimeParameterName, "F" }
            };
            _ticketConfig = new TicketConfig(reason,
                                             ticketInfo.Parameters.Keys.ToArray(),
                                             formats);
            _existingParameters = ticketInfo.Parameters;
            var result = await _ticketProvider.CreateAsync(reason, userId, null);

            result.Should().NotBeNull();
            result.Reason.Should().Be(reason);
            result.Parameters.Should().NotBeNullOrEmpty();
            using var _ = new AssertionScope();

            result.Parameters.Should()
                  .ContainKey(ticketInfo.DateTimeXpath)
                  .WhoseValue.Should()
                  .Be(ticketInfo.DateTimeXpathValue.ToString(formats[ticketInfo.DateTimeXpath]));

            result.Parameters.Should()
                  .ContainKey(ticketInfo.DateTimeParameterName)
                  .WhoseValue.Should()
                  .Be(ticketInfo.DateTimeParameterValue.ToString(formats[ticketInfo.DateTimeParameterName]));
        }

        [Theory]
        public async Task ShouldThrowException_WhenNotAllParametersExists(TicketInfo ticketInfo, Guid userId)
        {
            var reason = "test template";
            var absentParameter1 = "AbsentParameter1";
            var absentParameter2 = "AbsentParameter2";
            var requiredParameters = ticketInfo.Parameters.Keys.Append(absentParameter1).Append(absentParameter2).ToArray();
            _ticketConfig = new TicketConfig(reason,
                                             requiredParameters,
                                             new Dictionary<string, string>());
            _existingParameters = ticketInfo.Parameters;
            Func<Task> act = () => _ticketProvider.CreateAsync(reason, userId, null);

            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().ContainAll(absentParameter1, absentParameter2);
            ex.Which.Message.Should().ContainAll(reason);
        }
    }
}