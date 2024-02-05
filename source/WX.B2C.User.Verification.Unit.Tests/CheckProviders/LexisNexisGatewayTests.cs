using System.Threading.Tasks;
using FluentAssertions;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.LexisNexis;
using WX.B2C.User.Verification.LexisNexis.Configurations;
using WX.B2C.User.Verification.LexisNexis.Mappers;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.LexisNexis.Processors;
using WX.B2C.User.Verification.LexisNexis.Runners;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests
{
    public class LexisNexisGatewayTests
    {
        private readonly IRdpApiClientFactory _clientFactory;
        private readonly FraudScreeningCheckProcessor _checkResultValidator;
        private readonly ICheckRequestMapper _checkMapper;

        public async Task Should()
        {
            // Arrange
            var configuration = new FraudScreeningCheckConfiguration();
            var checkData = new FraudScreeningCheckData();
            var provider = new FraudScreeningCheckRunner(_clientFactory, _checkResultValidator, _checkMapper, configuration);

            // Act & Assert
            var runningContext = await provider.RunAsync(checkData);

            var processingContext = new CheckProcessingContext(new CheckExternalDataDto());
            var checkResult = await provider.GetResultAsync(processingContext);
            checkResult.Should().NotBeNull();
        }
    }
}
