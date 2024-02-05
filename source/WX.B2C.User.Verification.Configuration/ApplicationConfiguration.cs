using WX.B2C.User.Verification.Configuration.Models;
using WX.Configuration.Admin.Data;
using WX.Configuration.Admin.Interface;
using WX.Configuration.Contracts.Data;
using WX.Configuration.Contracts.Interface;

namespace WX.B2C.User.Verification.Configuration
{
    public interface IApplicationConfiguration
        : ILoggerConfiguration,
          IHostEnvironmentInformation,
          IApplicationInformation,
          IEndpointsInformation,
          IAdminConfiguration,
          IKeyVaultConfiguration,
          IServiceConfiguration<ServiceConfiguration>
    {

    }

    public class ServiceConfiguration
    {
        public int Version { get; set; }

        public CountriesConfiguration Countries { get; set; }

        public TicketsConfiguration Tickets { get; set; }

        public RegionActions[] Actions { get; set; }

        public HostLogLevel[] HostsLogLevel { get; set; }

        public ReminderSpan[] ReminderSpans { get; set; }
    }

    public class CountriesConfiguration
    {
        public string[] SupportedCountries { get; set; }

        public SupportedStates[] SupportedStates { get; set; }

        public Region[] Regions { get; set; }
        
        public string[] BlacklistedCountries { get; set; }
    }
    
    public class TicketsConfiguration
    {
        public Ticket[] Tickets { get; set; }        
        
        public ParametersMapping[] TicketParametersMapping { get; set; }
    }

    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public LoggerData Logger { get; set; }

        public EnvironmentData Environment { get; set; }

        public ApplicationData Application { get; set; }

        public EndpointData[] Endpoints { get; set; }

        public KeyVaultData KeyVaultConfigSection { get; set; }

        public AdminConfigurationData Admin { get; set; }
        
        public ServiceConfiguration Service { get; set; }

        //FIX Add host settings config
        //public HostConfig AppConfigSection { get; set; }

        // public HostConfig HostConfig => AppConfigSection;
        
    }
}