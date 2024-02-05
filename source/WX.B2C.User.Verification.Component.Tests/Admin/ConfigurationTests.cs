using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Factories;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    [TestFixture]
    internal class ConfigurationTests : BaseComponentTest
    {
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;

        private ServiceConfigurationDto _preservedConfiguration;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            Arb.Register<UpdateConfigurationRequestArbitrary>();
            
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();
            
            await PreserveServiceConfigurationAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await RestoreServiceConfigurationAsync();
        }

        [Theory]
        public async Task ShouldUpdateCountriesConfiguration(UpdateConfigurationRequest request)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Arrange
            var configuration = await client.Configuration.GetAsync();
            var expectedResult = new ServiceConfigurationDto
            {
                SeedVersion = configuration.SeedVersion,
                BlacklistedCountries = request.BlacklistedCountries ?? configuration.BlacklistedCountries,
                SupportedCountries = request.SupportedCountries ?? configuration.SupportedCountries,
                SupportedStates = request.SupportedStates ?? configuration.SupportedStates,
                Regions = request.Regions ?? configuration.Regions,
                Tickets = request.Tickets ?? configuration.Tickets,
                TicketParametersMapping = request.TicketParametersMapping ?? configuration.TicketParametersMapping,
                Actions = request.RegionActions ?? configuration.Actions,
                HostLogLevels = request.HostLogLevels ?? configuration.HostLogLevels
            };

            // Act
            var result = await client.Configuration.UpdateAsync(request);

            // Assert
            var actualConfiguration = await client.Configuration.GetAsync();

            result.Should().BeEquivalentTo(expectedResult);
            result.Should().BeEquivalentTo(actualConfiguration);
        }

        private async Task PreserveServiceConfigurationAsync()
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            _preservedConfiguration = await client.Configuration.GetAsync();
            Console.WriteLine(JsonConvert.SerializeObject(_preservedConfiguration, Formatting.Indented));
        }

        private async Task RestoreServiceConfigurationAsync()
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var countriesConfigurationPatch = new UpdateConfigurationRequest(
                _preservedConfiguration.SupportedCountries,
                _preservedConfiguration.SupportedStates,
                _preservedConfiguration.Regions,
                _preservedConfiguration.Actions,
                _preservedConfiguration.BlacklistedCountries,
                _preservedConfiguration.Tickets,
                _preservedConfiguration.TicketParametersMapping,
                _preservedConfiguration.HostLogLevels);
            await client.Configuration.UpdateAsync(countriesConfigurationPatch);
        }
    }
}
