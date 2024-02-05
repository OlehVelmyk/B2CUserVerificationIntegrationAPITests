using System;
using System.Globalization;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Configuration.Models;
using WX.Configuration.Contracts.Interface;

namespace WX.B2C.User.Verification.Configuration
{
    public interface IConfigurationManager
    {
        ServiceConfiguration Get();

        Task UpdateAsync(ConfigurationPatch configurationPatch);
    }

    internal class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationManager<IApplicationConfiguration> _configurationManager;
        private readonly IApplicationConfiguration _configuration;

        public ConfigurationManager(IApplicationConfiguration configuration,
                                    IConfigurationManager<IApplicationConfiguration> configurationManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
        }

        public ServiceConfiguration Get() => _configuration.Service;

        public async Task UpdateAsync(ConfigurationPatch configurationPatch)
        {
            if (configurationPatch is null)
                throw new ArgumentNullException(nameof(configurationPatch));

            ApplyPatch(_configuration.Service, configurationPatch);
            await _configurationManager.PersistConfiguration(_configuration, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }

        private static void ApplyPatch(ServiceConfiguration serviceConfiguration, ConfigurationPatch patch)
        {
            var (countriesConfiguration, ticketsConfiguration) = (serviceConfiguration.Countries, serviceConfiguration.Tickets);

            patch.BlacklistedCountries
                 .MatchSome(result => countriesConfiguration.BlacklistedCountries = result);
            patch.SupportedCountries
                 .MatchSome(result => countriesConfiguration.SupportedCountries = result);
            patch.SupportedStates
                 .MatchSome(result => countriesConfiguration.SupportedStates = result);
            patch.Regions
                 .MatchSome(result => countriesConfiguration.Regions = result);
            patch.RegionActions
                 .MatchSome(result => serviceConfiguration.Actions = result);
            patch.Tickets
                 .MatchSome(result => ticketsConfiguration.Tickets = result);
            patch.TicketParametersMapping
                 .MatchSome(result => ticketsConfiguration.TicketParametersMapping = result);
            patch.HostLogLevels
                 .MatchSome(result => serviceConfiguration.HostsLogLevel = result);
        }
    }
}
