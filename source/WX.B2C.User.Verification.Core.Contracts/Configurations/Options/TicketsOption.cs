using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class TicketParametersMappingOption : Option
    {
        private readonly IDictionary<string, ParameterMapping> _mapping;
        
        public TicketParametersMappingOption(IDictionary<string, ParameterMapping> mapping)
        {
            if (mapping?.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(mapping));

            _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }

        public ParameterMapping GetMapping(string parameter)
        {
            if (!_mapping.TryGetValue(parameter, out var source))
                throw new ArgumentOutOfRangeException(nameof(parameter), parameter, "Not found mapping for parameter");

            return source;
        }
    }
    
    public class ParameterMapping
    {
        public string Name { get; }

        public string Source { get; }

        public ParameterMapping(string name, string source)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }

    
    public class TicketsOption : Option
    {
        private readonly Dictionary<string, TicketConfig> _ticketsParameters;

        public TicketsOption(TicketConfig[] tickets)
        {
            if (tickets == null)
                throw new ArgumentNullException(nameof(tickets));
            if (tickets.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(tickets));

            _ticketsParameters = tickets.ToDictionary(parameters => parameters.Reason, parameters => parameters);
        }

        public TicketConfig Get(string reason)
        {
            if (!_ticketsParameters.TryGetValue(reason, out var config))
                throw new ArgumentOutOfRangeException(nameof(reason), reason, $"Not found config for ticket with reason: {reason}");

            return config;
        }
    }

    public class TicketConfig
    {
        public string Reason { get; }

        public string[] Parameters { get; }

        public Dictionary<string, string> Formats { get; }

        public TicketConfig(string reason,
                                string[] parameters,
                                Dictionary<string, string> formats)
        {
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Formats = formats ?? throw new ArgumentNullException(nameof(formats));
        }
    }
}