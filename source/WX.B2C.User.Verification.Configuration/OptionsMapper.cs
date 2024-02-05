using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Configuration.Models;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using Action = WX.B2C.User.Verification.Configuration.Models.Action;

namespace WX.B2C.User.Verification.Configuration
{
    internal interface IOptionsMapper
    {
        SupportedStatesOption Map(SupportedStates[] supportedStates);

        RegionsOption Map(Region[] regions);

        SupportedCountriesOption Map(string[] supportedCountries);

        ActionsOption Map(RegionActions[] actions);

        BlacklistedCountriesOption MapBlacklist(string[] blacklistedCountries);

        TicketsOption Map(Ticket[] tickets);

        TicketParametersMappingOption Map(ParametersMapping[] mapping);

        LogLevelOption Map(HostLogLevel[] hostsLogLevel);

        UserReminderOption Map(ReminderSpan[] reminderSpans);
    }

    internal class OptionsMapper : IOptionsMapper
    {
        public SupportedStatesOption Map(SupportedStates[] supportedStates)
        {
            if (supportedStates == null)
                throw new ArgumentNullException(nameof(supportedStates));

            return new(supportedStates.Select(Map));
        }

        public RegionsOption Map(Region[] regions)
        {
            if (regions == null)
                throw new ArgumentNullException(nameof(regions));

            return new(regions.Select(Map));
        }

        public SupportedCountriesOption Map(string[] supportedCountries)
        {
            if (supportedCountries == null)
                throw new ArgumentNullException(nameof(supportedCountries));

            return new(supportedCountries);
        }

        public ActionsOption Map(RegionActions[] actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            return new(actions.Select(Map));
        }

        public BlacklistedCountriesOption MapBlacklist(string[] blacklistedCountries)
        {
            if (blacklistedCountries == null)
                throw new ArgumentNullException(nameof(blacklistedCountries));

            return new(blacklistedCountries);
        }

        public TicketsOption Map(Ticket[] tickets)
        {
            if (tickets == null)
                throw new ArgumentNullException(nameof(tickets));

            return new TicketsOption(tickets.Select(Map).ToArray());

            TicketConfig Map(Ticket ticket) => 
                new(ticket.Reason, ticket.Parameters ?? Array.Empty<string>(), ticket.Formats ?? new Dictionary<string, string>());
        }

        public TicketParametersMappingOption Map(ParametersMapping[] mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            return new(mapping.ToDictionary(parametersMapping => parametersMapping.Name, Map));

            ParameterMapping Map(ParametersMapping parametersMapping) => 
                new(parametersMapping.Name, parametersMapping.Source);
        }

        public LogLevelOption Map(HostLogLevel[] hostsLogLevel)
        {
            if (hostsLogLevel == null)
                return new LogLevelOption();

            return new LogLevelOption(hostsLogLevel.ToDictionary(level => level.Host, level => level.Level));
        }

        public UserReminderOption Map(ReminderSpan[] reminderSpans)
        {
            if (reminderSpans == null)
                throw new ArgumentNullException(nameof(reminderSpans));
            if (reminderSpans.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(reminderSpans));

            var spans = reminderSpans.OrderBy(span => span.Order)
                                     .Select(span => new ReminderSpanOption(span.Value, span.Unit))
                                     .ToArray();
            
            return new UserReminderOption(spans);
        }

        private static RegionActionsOption Map(RegionActions regionActions)
        {
            if (regionActions == null)
                throw new ArgumentNullException(nameof(regionActions));

            return new(regionActions.RegionType, regionActions.Region, regionActions.Actions.Select(Map));
        }

        private static ActionOption Map(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new(action.ActionType, action.XPath, action.Priority, action.Metadata);
        }

        private static CountrySupportedStatesOption Map(SupportedStates supportedStates)
        {
            if (supportedStates == null)
                throw new ArgumentNullException(nameof(supportedStates));

            return new(supportedStates.Country, supportedStates.States);
        }

        private static RegionOption Map(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return new(region.Name, region.Countries);
        }
    }
}