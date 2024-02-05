using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class ExcludedNamesOption : Option
    {
        public ExcludedNamesOption(IEnumerable<ExcludedNameOption> excludedNames)
        {
            if (excludedNames == null)
                throw new ArgumentNullException(nameof(excludedNames));

            Names = excludedNames.ToArray();
        }

        public IReadOnlyCollection<ExcludedNameOption> Names { get; }
    }

    public class ExcludedNameOption
    {
        public const string AnyNameKey = "*";
        
        public ExcludedNameOption(string firstName, string lastName)
        {
            if (firstName == AnyNameKey && lastName == AnyNameKey)
                throw new ArgumentException($"Both parameters {nameof(firstName)} and {nameof(lastName)} cannot be '{AnyNameKey}'.");

            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        public string FirstName { get; }

        public string LastName { get; }
    }
}
