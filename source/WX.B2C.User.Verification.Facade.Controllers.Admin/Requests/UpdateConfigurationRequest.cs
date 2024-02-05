using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UpdateConfigurationRequest
    {
        [NotRequired]
        public string[] SupportedCountries { get; set; }

        [NotRequired]
        public SupportedStatesDto[] SupportedStates { get; set; }

        [NotRequired]
        public RegionDto[] Regions { get; set; }

        [NotRequired]
        public RegionActionsDto[] RegionActions { get; set; }

        [NotRequired]
        public string[] BlacklistedCountries { get; set; }

        [NotRequired]
        public TicketDto[] Tickets { get; set; }

        [NotRequired]
        public ParametersMappingDto[] TicketParametersMapping { get; set; }

        [NotRequired]
        public HostLogLevelDto[] HostLogLevels { get; set; }
    }
}
