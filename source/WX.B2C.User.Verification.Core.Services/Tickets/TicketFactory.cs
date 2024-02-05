using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using Optional.Collections;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;

namespace WX.B2C.User.Verification.Core.Services.Tickets
{
    internal interface ITicketFactory
    {
        Task<TicketDto> CreateAsync(string reason,
                                    Guid userId,
                                    IReadOnlyDictionary<string, object> additionalParameters = null);
    }

    internal class TicketFactory : ITicketFactory
    {
        private readonly ITicketParametersReader _parametersReader;
        private readonly IOptionProvider<TicketsOption> _optionProvider;

        public TicketFactory(ITicketParametersReader parametersReader, IOptionProvider<TicketsOption> optionProvider)
        {
            _parametersReader = parametersReader ?? throw new ArgumentNullException(nameof(parametersReader));
            _optionProvider = optionProvider ?? throw new ArgumentNullException(nameof(optionProvider));
        }

        public async Task<TicketDto> CreateAsync(string reason,
                                                 Guid userId,
                                                 IReadOnlyDictionary<string, object> additionalParameters)
        {
            var ticketsOption = await _optionProvider.GetAsync();
            var config = ticketsOption.Get(reason);
            var parameters = await _parametersReader.ReadParametersAsync(userId, config.Parameters, additionalParameters);
            Validate(config, parameters);
            
            var formattedParameters = Format(parameters, config.Formats);
            return new TicketDto
            {
                Reason = reason,
                Parameters = formattedParameters
            };
        }

        private void Validate(TicketConfig config, IReadOnlyDictionary<string,object> existing)
        {
            var absent = config.Parameters.Except(existing.Keys).ToArray();
            if (absent.Length == 0)
                return;

            throw new InvalidOperationException(
            $"Not all parameters were read for ticket {config.Reason}. Absent parameters {string.Join(",", absent)}");
        }

        private IReadOnlyDictionary<string,string> Format(IReadOnlyDictionary<string,object> parameters, 
                                                          Dictionary<string,string> configFormats)
        {
            var result = new Dictionary<string, string>();

            foreach (var parameter in parameters)
            {
                var format = configFormats.GetValueOrNone(parameter.Key);
                result[parameter.Key] = Format(parameter.Value, format);
            }

            return result;
        }
        
        private string Format(object value, Option<string> parameterFormat) =>
            parameterFormat.Match(format => string.Format($"{{0:{format}}}",value), value.ToString);
    }
}