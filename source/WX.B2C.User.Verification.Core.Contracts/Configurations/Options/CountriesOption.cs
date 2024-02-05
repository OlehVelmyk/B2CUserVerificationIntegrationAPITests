using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class CountriesOption : Option
    {
        public IReadOnlyCollection<CountryOption> Countries { get; }

        public CountriesOption(IEnumerable<CountryOption> countries)
        {
            if (countries == null)
                throw new ArgumentNullException(nameof(countries));

            Countries = countries.ToArray();
        }
    }

    public class CountryOption
    {
        public CountryOption(string alpha2Code,
                             string alpha3Code,
                             string name,
                             IEnumerable<StateOption> states)
        {
            Alpha2Code = alpha2Code ?? throw new ArgumentNullException(nameof(alpha2Code));
            Alpha3Code = alpha3Code ?? throw new ArgumentNullException(nameof(alpha3Code)); 
            Name = name ?? throw new ArgumentNullException(nameof(name));
            States = states?.ToArray();
        }

        public string Alpha2Code { get; }

        public string Alpha3Code { get; }

        public string Name { get; }

        public IReadOnlyCollection<StateOption> States { get; }
    }

    public class StateOption
    {
        public StateOption(string code, string name)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Code { get; }

        public string Name { get; }
    }
}