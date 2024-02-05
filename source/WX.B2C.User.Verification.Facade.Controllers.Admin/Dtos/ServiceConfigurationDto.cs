using System.Collections.Generic;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class ServiceConfigurationDto
    {
        public int SeedVersion { get; set; }

        public string[] SupportedCountries { get; set; }

        public SupportedStatesDto[] SupportedStates { get; set; }

        public RegionDto[] Regions { get; set; }

        public string[] BlacklistedCountries { get; set; }

        public TicketDto[] Tickets { get; set; }

        public ParametersMappingDto[] TicketParametersMapping { get; set; }

        public RegionActionsDto[] Actions { get; set; }

        public HostLogLevelDto[] HostLogLevels { get; set; }
    }

    public class RegionActionsDto
    {
        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public ActionDto[] Actions { get; set; }
    }

    public class HostLogLevelDto
    {
        public string Host { get; set; }

        public LogEventLevel Level { get; set; }
    }

    public class ActionDto
    {
        public ActionType ActionType { get; set; }

        public string XPath { get; set; }

        public int Priority { get; set; }

        [NotRequired]
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class TicketDto
    {
        public string Reason { get; set; }

        [NotRequired]
        public string[] Parameters { get; set; }

        [NotRequired]
        public Dictionary<string, string> Formats { get; set; }
    }

    public class ParametersMappingDto
    {
        public string Name { get; set; }

        public string Source { get; set; }
    }

    public class SupportedStatesDto
    {
        public string Country { get; set; }

        public string[] States { get; set; }
    }

    public class RegionDto
    {
        public string Name { get; set; }

        public string[] Countries { get; set; }
    }
}
