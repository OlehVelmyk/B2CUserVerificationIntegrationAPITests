using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class RegionsOption : Option
    {
        public RegionsOption(IEnumerable<RegionOption> regions)
        {
            if (regions == null)
                throw new ArgumentNullException(nameof(regions));

            Regions = regions.ToArray();
        }

        public IReadOnlyCollection<RegionOption> Regions { get; }
    }

    public class RegionOption
    {
        public RegionOption(string name, IReadOnlyCollection<string> countries)
        {
            Name = name;
            Countries = countries;
        }

        public string Name { get; }

        public IReadOnlyCollection<string> Countries { get; }
    }
}