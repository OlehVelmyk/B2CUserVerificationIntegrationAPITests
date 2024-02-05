// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ServiceConfigurationDto
    {
        /// <summary>
        /// Initializes a new instance of the ServiceConfigurationDto class.
        /// </summary>
        public ServiceConfigurationDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ServiceConfigurationDto class.
        /// </summary>
        public ServiceConfigurationDto(int seedVersion, IList<string> supportedCountries, IList<SupportedStatesDto> supportedStates, IList<RegionDto> regions, IList<string> blacklistedCountries, IList<TicketDto> tickets, IList<ParametersMappingDto> ticketParametersMapping, IList<RegionActionsDto> actions, IList<HostLogLevelDto> hostLogLevels)
        {
            SeedVersion = seedVersion;
            SupportedCountries = supportedCountries;
            SupportedStates = supportedStates;
            Regions = regions;
            BlacklistedCountries = blacklistedCountries;
            Tickets = tickets;
            TicketParametersMapping = ticketParametersMapping;
            Actions = actions;
            HostLogLevels = hostLogLevels;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "seedVersion")]
        public int SeedVersion { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "supportedCountries")]
        public IList<string> SupportedCountries { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "supportedStates")]
        public IList<SupportedStatesDto> SupportedStates { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "regions")]
        public IList<RegionDto> Regions { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "blacklistedCountries")]
        public IList<string> BlacklistedCountries { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "tickets")]
        public IList<TicketDto> Tickets { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ticketParametersMapping")]
        public IList<ParametersMappingDto> TicketParametersMapping { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "actions")]
        public IList<RegionActionsDto> Actions { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hostLogLevels")]
        public IList<HostLogLevelDto> HostLogLevels { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (SupportedCountries == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SupportedCountries");
            }
            if (SupportedStates == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SupportedStates");
            }
            if (Regions == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Regions");
            }
            if (BlacklistedCountries == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "BlacklistedCountries");
            }
            if (Tickets == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Tickets");
            }
            if (TicketParametersMapping == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TicketParametersMapping");
            }
            if (Actions == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Actions");
            }
            if (HostLogLevels == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "HostLogLevels");
            }
            if (SupportedStates != null)
            {
                foreach (var element in SupportedStates)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (Regions != null)
            {
                foreach (var element1 in Regions)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
                    }
                }
            }
            if (Tickets != null)
            {
                foreach (var element2 in Tickets)
                {
                    if (element2 != null)
                    {
                        element2.Validate();
                    }
                }
            }
            if (TicketParametersMapping != null)
            {
                foreach (var element3 in TicketParametersMapping)
                {
                    if (element3 != null)
                    {
                        element3.Validate();
                    }
                }
            }
            if (Actions != null)
            {
                foreach (var element4 in Actions)
                {
                    if (element4 != null)
                    {
                        element4.Validate();
                    }
                }
            }
            if (HostLogLevels != null)
            {
                foreach (var element5 in HostLogLevels)
                {
                    if (element5 != null)
                    {
                        element5.Validate();
                    }
                }
            }
        }
    }
}
