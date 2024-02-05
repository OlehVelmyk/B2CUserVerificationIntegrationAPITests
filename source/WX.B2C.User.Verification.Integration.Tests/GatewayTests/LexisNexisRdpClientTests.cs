using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.TestCases;
using WX.B2C.User.Verification.LexisNexis;
using WX.B2C.User.Verification.LexisNexis.IoC;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    [TestFixture]
    internal class LexisNexisRdpClientTests : BaseIntegrationTest
    {
        private IRdpApiClientFactory _rdpClientFactory;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            RegisterKeyVault<ILexisNexisRdpKeyVault>(containerBuilder);
            containerBuilder.RegisterLexisNexisProvider(_ => false);
            var operationContextProvider = Substitute.For<IOperationContextProvider>();
            operationContextProvider.GetContextOrDefault().Returns(OperationContext.Create(Guid.NewGuid(), null, Guid.NewGuid(), "Test"));
            containerBuilder.Register(_ => operationContextProvider).As<IOperationContextProvider>();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _rdpClientFactory = Resolve<IRdpApiClientFactory>();

            Arb.Register<InitiateWorkflowRequestArbitrary>();
        }

        [Theory]
        public async Task ShouldReturnResponse_WhenInitiateWorkflow(InitiateWorkflowRequest request)
        {
            // Arrange
            request.Settings = _rdpClientFactory.GetSettings();

            // Act
            using var client = _rdpClientFactory.Create();
            var response = await client.Workflow.InitiateAsync(request);

            // Assert
            response.Should().NotBeNull();
        }

        [TestCaseSource(typeof(InitiateWorkflowContainers))]
        public async Task InitiateAsync_ShouldReturnExpectedCviAndHri(InitiateWorkflowContainer container)
        {
            // Arrange
            var request = container.Request;
            request.Settings = _rdpClientFactory.GetSettings();

            // Act
            using var client = _rdpClientFactory.Create();
            var response = await client.Workflow.InitiateAsync(request);

            // Assert
            response.Should().NotBeNull();
            var passThrough = response?.PassThroughs?.FirstOrDefault();
            passThrough.Should().NotBeNull();
            passThrough.Data.Should().NotBeNullOrEmpty();
            var instantIDResponseExWrapper = JsonConvert.DeserializeObject<InstantIDResponseExWrapper>(passThrough.Data);
            var cv = instantIDResponseExWrapper
                     ?.InstantIDResponseEx
                     ?.Response
                     ?.Result
                     ?.ComprehensiveVerification;
            cv.Should().NotBeNull();
            cv.ComprehensiveVerificationIndex.Should().Be(container.Cvi);
            cv.RiskIndicators?.RiskIndicator.FirstOrDefault()?.RiskCode.Should().Be(container.Hri);
        }
    }
}