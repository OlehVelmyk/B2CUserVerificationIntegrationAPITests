using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.Configuration.Contracts.Interface;

namespace WX.B2C.User.Verification.Configuration
{
    internal class OptionsProvider
        : IOptionProvider<SupportedStatesOption>,
          IOptionProvider<RegionsOption>,
          IOptionProvider<SupportedCountriesOption>,
          IOptionProvider<ActionsOption>,
          IOptionProvider<BlacklistedCountriesOption>,
          IOptionProvider<TicketsOption>,
          IOptionProvider<TicketParametersMappingOption>,
          IOptionProvider<LogLevelOption>,
          IOptionProvider<UserReminderOption>
    {
        private readonly IOptionsMapper _optionsMapper;
        private readonly IServiceConfiguration<ServiceConfiguration> _configuration;

        public OptionsProvider(IServiceConfiguration<ServiceConfiguration> configuration, IOptionsMapper optionsMapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _optionsMapper = optionsMapper ?? throw new ArgumentNullException(nameof(optionsMapper));
        }

        Task<SupportedStatesOption> IOptionProvider<SupportedStatesOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Countries.SupportedStates));

        Task<RegionsOption> IOptionProvider<RegionsOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Countries.Regions));

        Task<SupportedCountriesOption> IOptionProvider<SupportedCountriesOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Countries.SupportedCountries));

        Task<ActionsOption> IOptionProvider<ActionsOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Actions));

        Task<BlacklistedCountriesOption> IOptionProvider<BlacklistedCountriesOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.MapBlacklist(_configuration.Service.Countries.BlacklistedCountries));

        Task<TicketsOption> IOptionProvider<TicketsOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Tickets.Tickets));
        
        Task<TicketParametersMappingOption> IOptionProvider<TicketParametersMappingOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.Tickets.TicketParametersMapping));

        public Task<LogLevelOption> GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration?.Service?.HostsLogLevel));

        Task<UserReminderOption> IOptionProvider<UserReminderOption>.GetAsync() =>
            Task.FromResult(_optionsMapper.Map(_configuration.Service.ReminderSpans));
    }
}