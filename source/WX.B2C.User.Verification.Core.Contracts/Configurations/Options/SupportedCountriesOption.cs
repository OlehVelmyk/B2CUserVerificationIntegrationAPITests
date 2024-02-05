﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class SupportedCountriesOption : Option
    {
        public SupportedCountriesOption(IEnumerable<string> countries)
        {
            if (countries == null)
                throw new ArgumentNullException(nameof(countries));

            Countries = countries.ToHashSet();
        }

        public IReadOnlyCollection<string> Countries { get; }
    }
}