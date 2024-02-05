using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Services.Tickets;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;
using TicketInfo = WX.B2C.User.Verification.Unit.Tests.Arbitraries.TicketInfo;

namespace WX.B2C.User.Verification.Unit.Tests.TicketProviders
{
    internal class TicketParametersReaderTests
    {
        private TicketParametersReader _ticketParametersReader;
        private IProfileDataCollection _profileDataCollection;
        private IOperationContextProvider _operationContextProvider;
        private ISystemClock _systemClock;
        private IExternalProfileStorage _externalProfileStorage;
        private IExternalLinkProvider _externalLinkProvider;
        private IProfileProviderFactory _profileProviderFactory;
        private IOptionProvider<TicketParametersMappingOption> _optionProvider;
        private Dictionary<string, string> _mappingOption = new() /*{ { "dummyParameterName", "dummySourceName" } }*/;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Arb.Register<TwoDifferentArbitrary<NonNull<string>>>();
            Arb.Register<TicketInfoArbitrary>();
        }
        
        [SetUp]
        public void Setup()
        {
            _profileDataCollection = Substitute.For<IProfileDataCollection>();

            var profileDataProvider = Substitute.For<IProfileDataProvider>();
            profileDataProvider.ReadAsync(null).ReturnsForAnyArgs(_ => _profileDataCollection);

            _profileProviderFactory = Substitute.For<IProfileProviderFactory>();
            _profileProviderFactory.Create(Arg.Any<Guid>()).Returns(profileDataProvider);

            _operationContextProvider = Substitute.For<IOperationContextProvider>();
            _externalProfileStorage = Substitute.For<IExternalProfileStorage>();
            _externalLinkProvider = Substitute.For<IExternalLinkProvider>();

            _systemClock = Substitute.For<ISystemClock>();

            _optionProvider = Substitute.For<IOptionProvider<TicketParametersMappingOption>>();
            _optionProvider.GetAsync()
                           .Returns(_ => Task.FromResult(new TicketParametersMappingOption(
                                                         _mappingOption.ToDictionary(
                                                         pair => pair.Key,
                                                         pair => new ParameterMapping(pair.Key, pair.Value)))));
            
            _ticketParametersReader = new TicketParametersReader(_profileProviderFactory,
                                                                 _optionProvider,
                                                                 _operationContextProvider,
                                                                 _externalProfileStorage,
                                                                 _externalLinkProvider,
                                                                 _systemClock);
        }

        [Theory]
        public async Task ShouldReadParameters(TicketInfo ticketInfo, Guid userId, TwoDifferent<NonNull<string>> parameters)
        {
            _mappingOption[parameters.Item1.Item] = ticketInfo.StringXpath;
            _mappingOption[parameters.Item2.Item] = ticketInfo.DateTimeXpath;
            
            _profileDataCollection[ticketInfo.StringXpath].Returns(ticketInfo.StringXpathValue);
            _profileDataCollection[ticketInfo.DateTimeXpath].Returns(ticketInfo.DateTimeXpathValue);
            
            var requestedParameters = new[]
            {
                parameters.Item1.Item,
                ticketInfo.DateTimeParameterName,
                parameters.Item2.Item,
                ticketInfo.StringParameterName,
            };
            var result = await _ticketParametersReader.ReadParametersAsync(userId, requestedParameters, BuildParametersValues(ticketInfo));

            result.Should().NotBeNull();
            result.Should().ContainKey(parameters.Item1.Item).WhoseValue.Should().Be(ticketInfo.StringXpathValue);
            result.Should().ContainKey(parameters.Item2.Item).WhoseValue.Should().Be(ticketInfo.DateTimeXpathValue);
            result.Should().ContainKey(ticketInfo.StringParameterName).WhoseValue.Should().Be(ticketInfo.StringParameterValue);
            result.Should().ContainKey(ticketInfo.DateTimeParameterName).WhoseValue.Should().Be(ticketInfo.DateTimeParameterValue);
        }

        [Theory]
        public async Task ShouldReadCorrelationId(Guid userId, Guid correlationId, NonNull<string> parameter)
        {
            //Arrange
            var parameterName = parameter.Item;
            _mappingOption[parameterName] = TemplateParameters.CorrelationId;
            
            _operationContextProvider.GetContextOrDefault().Returns(OperationContext.Create(correlationId, null, Guid.Empty, "test"));

            //Act
            var result = await _ticketParametersReader.ReadParametersAsync(userId, new[] { parameterName }, null);

            //Assert
            result.Should().ContainKey(parameterName).WhoseValue.Should().Be(correlationId.ToString());
        }

        [Theory]
        public async Task ShouldReadCurrentDateTime(Guid userId, DateTime currentDate, NonNull<string> parameterName)
        {
            _systemClock.GetDate().Returns(currentDate);
            _mappingOption[parameterName.Item] = TemplateParameters.CurrentDateTime;

            var result = await _ticketParametersReader.ReadParametersAsync(userId, new[] { parameterName.Item }, null);

            result.Should().ContainKey(parameterName.Item).WhoseValue.Should().Be(currentDate);
        }

        [Theory]
        public async Task ShouldReadPassFortLink(Guid userId, string profileId, string link, NonNull<string> parameterName)
        {
            _mappingOption[parameterName.Item] = TemplateParameters.PassFortLink;           
            _externalProfileStorage.GetExternalIdAsync(userId, ExternalProviderType.PassFort).Returns(profileId);
            _externalLinkProvider.Get(profileId, ExternalProviderType.PassFort).Returns(link);

            var result = await _ticketParametersReader.ReadParametersAsync(userId, new[] { parameterName.Item }, null);

            result.Should().ContainKey(parameterName.Item).WhoseValue.Should().Be(link);
        }

        [Theory]
        public async Task ShouldReadUserId(Guid userId)
        {
            var result = await _ticketParametersReader.ReadParametersAsync(userId, new[] { TemplateParameters.UserId }, null);

            result.Should().ContainKey(TemplateParameters.UserId).WhoseValue.Should().Be(userId);
        }

        [Theory]
        public async Task ShouldNotReadParameters_WhichAreProvided(Guid userId, XPath xPath)
        {
            var requestedParameters = new[]
            {
                TemplateParameters.PassFortLink,
                TemplateParameters.CorrelationId,
                TemplateParameters.CurrentDateTime,

                xPath,
            };
            var providedParameters = requestedParameters.ToDictionary(parameterName => parameterName, s => (object) s);

            var result = await _ticketParametersReader.ReadParametersAsync(userId, requestedParameters, providedParameters);

            using var _ = new AssertionScope();
            foreach (var providedParameter in providedParameters)
            {
                result.Should().ContainKey(providedParameter.Key).WhoseValue.Should().Be(providedParameter.Value);
            }

            await _externalProfileStorage.DidNotReceiveWithAnyArgs().GetExternalIdAsync(Arg.Any<Guid>(), Arg.Any<ExternalProviderType>());
            _externalLinkProvider.DidNotReceiveWithAnyArgs().Get(Arg.Any<string>(), Arg.Any<ExternalProviderType>());
            _operationContextProvider.DidNotReceiveWithAnyArgs().GetContextOrDefault();
            _systemClock.DidNotReceiveWithAnyArgs().GetDate();
            _profileProviderFactory.DidNotReceiveWithAnyArgs().Create(Arg.Any<Guid>());
        }

        private IReadOnlyDictionary<string, object> BuildParametersValues(TicketInfo ticketInfo) =>
            new Dictionary<string, object>
            {
                [ticketInfo.StringParameterName] = ticketInfo.StringParameterValue,
                [ticketInfo.DateTimeParameterName] = ticketInfo.DateTimeParameterValue
            };
    }
}