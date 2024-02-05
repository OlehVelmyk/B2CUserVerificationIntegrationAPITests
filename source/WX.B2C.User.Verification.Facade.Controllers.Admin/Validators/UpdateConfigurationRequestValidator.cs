using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class UpdateConfigurationRequestValidator : BaseRequestValidator<UpdateConfigurationRequest>
    {
        public UpdateConfigurationRequestValidator()
        {
            RuleFor(request => request).Custom(ValidateProperties);
        }

        private static void ValidateProperties(UpdateConfigurationRequest request,
                                               ValidationContext<UpdateConfigurationRequest> context)
        {
            var properties = new Dictionary<string, object>
            {
                [nameof(UpdateConfigurationRequest.BlacklistedCountries)] = request.BlacklistedCountries,
                [nameof(UpdateConfigurationRequest.SupportedCountries)] = request.SupportedCountries,
                [nameof(UpdateConfigurationRequest.SupportedStates)] = request.SupportedStates,
                [nameof(UpdateConfigurationRequest.Regions)] = request.Regions,
                [nameof(UpdateConfigurationRequest.RegionActions)] = request.RegionActions,
                [nameof(UpdateConfigurationRequest.Tickets)] = request.Tickets,
                [nameof(UpdateConfigurationRequest.TicketParametersMapping)] = request.TicketParametersMapping,
                [nameof(UpdateConfigurationRequest.HostLogLevels)] = request.HostLogLevels
            };

            var allPropertiesUndefined = properties.All(property => property.Value is null);
            if (allPropertiesUndefined)
                context.AddFailure(nameof(request), "At least one property should be defined.");
        }
    }
}
