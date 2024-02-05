using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class SupportedStatesOption : Option
    {
        public SupportedStatesOption(IEnumerable<CountrySupportedStatesOption> countriesSupportedStates)
        {
            if (countriesSupportedStates == null)
                throw new ArgumentNullException(nameof(countriesSupportedStates));

            CountrySupportedStates = countriesSupportedStates.ToDictionary(option => option.Country, option => option.States);
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> CountrySupportedStates { get; }
    }

    public class CountrySupportedStatesOption
    {
        public CountrySupportedStatesOption(string country, IEnumerable<string> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            Country = country ?? throw new ArgumentNullException(nameof(country));
            States = states.ToHashSet();
        }

        public string Country { get; }

        public IReadOnlyCollection<string> States { get; }
    }
}