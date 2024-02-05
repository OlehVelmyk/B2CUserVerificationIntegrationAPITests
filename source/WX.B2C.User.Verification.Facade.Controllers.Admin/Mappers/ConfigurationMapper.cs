using System;
using Optional;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using ConfigModels = WX.B2C.User.Verification.Configuration.Models;
using Configs = WX.B2C.User.Verification.Configuration;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IConfigurationMapper
    {
        ServiceConfigurationDto Map(Configs.ServiceConfiguration configuration);

        ConfigModels.ConfigurationPatch Map(UpdateConfigurationRequest configurationDto);
    }

    internal class ConfigurationMapper : IConfigurationMapper
    {
        public ServiceConfigurationDto Map(Configs.ServiceConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return new ServiceConfigurationDto
            {
                SeedVersion = configuration.Version,
                Actions = configuration.Actions?.MapToArray(Map),
                BlacklistedCountries = configuration.Countries.BlacklistedCountries,
                SupportedCountries = configuration.Countries.SupportedCountries,
                Regions = configuration.Countries.Regions?.MapToArray(Map),
                SupportedStates = configuration.Countries.SupportedStates?.MapToArray(Map),
                Tickets = configuration.Tickets?.Tickets?.MapToArray(Map),
                TicketParametersMapping = configuration.Tickets?.TicketParametersMapping?.MapToArray(Map),
                HostLogLevels = configuration.HostsLogLevel?.MapToArray(Map)
            };
        }

        public ConfigModels.ConfigurationPatch Map(UpdateConfigurationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new ConfigModels.ConfigurationPatch
            {
                BlacklistedCountries = request.BlacklistedCountries.SomeNotNull(),
                SupportedCountries = request.SupportedCountries.SomeNotNull(),
                SupportedStates = (request.SupportedStates?.MapToArray(Map)).SomeNotNull(),
                Regions = (request.Regions?.MapToArray(Map)).SomeNotNull(),
                RegionActions = (request.RegionActions?.MapToArray(Map)).SomeNotNull(),
                Tickets = (request.Tickets?.MapToArray(Map)).SomeNotNull(),
                TicketParametersMapping = (request.TicketParametersMapping?.MapToArray(Map)).SomeNotNull(),
                HostLogLevels = (request.HostLogLevels?.MapToArray(Map)).SomeNotNull()
            };
        }

        private static RegionActionsDto Map(ConfigModels.RegionActions regionActions)
        {
            if (regionActions == null)
                throw new ArgumentNullException(nameof(regionActions));

            return new RegionActionsDto
            {
                Region = regionActions.Region,
                RegionType = regionActions.RegionType,
                Actions = regionActions.Actions?.MapToArray(Map)
            };
        }

        private static ConfigModels.RegionActions Map(RegionActionsDto regionActions)
        {
            if (regionActions == null)
                throw new ArgumentNullException(nameof(regionActions));

            return new ConfigModels.RegionActions
            {
                Region = regionActions.Region,
                RegionType = regionActions.RegionType,
                Actions = regionActions.Actions?.MapToArray(Map)
            };
        }

        private static ActionDto Map(ConfigModels.Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new ActionDto
            {
                ActionType = action.ActionType,
                Priority = action.Priority,
                XPath = action.XPath,
                Metadata = action.Metadata
            };
        }

        private static ConfigModels.Action Map(ActionDto action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new ConfigModels.Action
            {
                ActionType = action.ActionType,
                Priority = action.Priority,
                XPath = action.XPath,
                Metadata = action.Metadata
            };
        }

        private static RegionDto Map(ConfigModels.Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return new RegionDto
            {
                Name = region.Name,
                Countries = region.Countries
            };
        }

        private static ConfigModels.Region Map(RegionDto region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return new ConfigModels.Region
            {
                Name = region.Name,
                Countries = region.Countries
            };
        }

        private static SupportedStatesDto Map(ConfigModels.SupportedStates supportedStates)
        {
            if (supportedStates == null)
                throw new ArgumentNullException(nameof(supportedStates));

            return new SupportedStatesDto
            {
                Country = supportedStates.Country,
                States = supportedStates.States
            };
        }

        private static ConfigModels.SupportedStates Map(SupportedStatesDto supportedStates)
        {
            if (supportedStates == null)
                throw new ArgumentNullException(nameof(supportedStates));

            return new ConfigModels.SupportedStates
            {
                Country = supportedStates.Country,
                States = supportedStates.States
            };
        }

        private static TicketDto Map(ConfigModels.Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            return new TicketDto
            {
                Reason = ticket.Reason,
                Parameters = ticket.Parameters,
                Formats = ticket.Formats
            };
        }

        private static ConfigModels.Ticket Map(TicketDto ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            return new ConfigModels.Ticket
            {
                Reason = ticket.Reason,
                Parameters = ticket.Parameters,
                Formats = ticket.Formats
            };
        }

        private static ParametersMappingDto Map(ConfigModels.ParametersMapping parametersMapping)
        {
            if (parametersMapping == null)
                throw new ArgumentNullException(nameof(parametersMapping));

            return new ParametersMappingDto
            {
                Name = parametersMapping.Name,
                Source = parametersMapping.Source
            };
        }

        private static ConfigModels.ParametersMapping Map(ParametersMappingDto parametersMapping)
        {
            if (parametersMapping == null)
                throw new ArgumentNullException(nameof(parametersMapping));

            return new ConfigModels.ParametersMapping
            {
                Name = parametersMapping.Name,
                Source = parametersMapping.Source
            };
        }

        private static HostLogLevelDto Map(ConfigModels.HostLogLevel hostLogLevel)
        {
            if (hostLogLevel == null)
                throw new ArgumentNullException(nameof(hostLogLevel));

            return new HostLogLevelDto
            {
                Host = hostLogLevel.Host,
                Level = hostLogLevel.Level
            };
        }

        private static ConfigModels.HostLogLevel Map(HostLogLevelDto hostLogLevel)
        {
            if (hostLogLevel == null)
                throw new ArgumentNullException(nameof(hostLogLevel));

            return new ConfigModels.HostLogLevel
            {
                Host = hostLogLevel.Host,
                Level = hostLogLevel.Level
            };
        }
    }
}
