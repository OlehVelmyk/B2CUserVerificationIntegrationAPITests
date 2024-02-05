using Optional;

namespace WX.B2C.User.Verification.Configuration.Models
{
    public sealed class ConfigurationPatch
    {
        public Option<string[]> SupportedCountries { get; set; }

        public Option<string[]> BlacklistedCountries { get; set; }

        public Option<SupportedStates[]> SupportedStates { get; set; }

        public Option<Region[]> Regions { get; set; }

        public Option<Ticket[]> Tickets { get; set; }

        public Option<ParametersMapping[]> TicketParametersMapping { get; set; }

        public Option<RegionActions[]> RegionActions { get; set; }

        public Option<HostLogLevel[]> HostLogLevels { get; set; }
    }
}
