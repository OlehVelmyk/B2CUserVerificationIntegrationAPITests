using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class SpecificCountriesOption : Option
    {
        public IReadOnlyCollection<SpecificCountryOption> Countries { get; }

        public SpecificCountriesOption(IEnumerable<SpecificCountryOption> countries)
        {
            if (countries == null)
                throw new ArgumentNullException(nameof(countries));

            Countries = countries.ToArray();
        }
    }

    public class SpecificCountryOption
    {
        public SpecificCountryOption(string[] icaoCodes,
                                     string alpha2Code,
                                     string alpha3Code,
                                     string name)
        {
            IcaoCodes = icaoCodes ?? throw new ArgumentNullException(nameof(icaoCodes));
            Alpha2Code = alpha2Code ?? throw new ArgumentNullException(nameof(alpha2Code));
            Alpha3Code = alpha3Code ?? throw new ArgumentNullException(nameof(alpha3Code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string[] IcaoCodes { get; }
        
        public string Alpha2Code { get; }

        public string Alpha3Code { get; }

        public string Name { get; }
    }
}
