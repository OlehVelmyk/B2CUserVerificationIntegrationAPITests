using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class MatchCountryConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new MatchCountryCondition((string)config);
    }

    internal class MatchCountryCondition : ICondition
    {
        //TODO think how to fix magic string
        public const string MatchCountryRequiredData = "MatchedCountries";

        private readonly string _country;

        public MatchCountryCondition(string country)
        {
            _country = country ?? throw new ArgumentNullException(nameof(country));
        }

        public static string[] RequiredData => new[] { MatchCountryRequiredData };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var countries = context.Get<string[]>(MatchCountryRequiredData);
            return countries.Contains(_country);
        }
    }
}