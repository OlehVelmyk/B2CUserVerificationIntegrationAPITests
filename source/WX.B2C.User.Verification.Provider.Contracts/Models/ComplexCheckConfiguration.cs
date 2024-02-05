using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public class ComplexCheckConfiguration : CheckProviderConfiguration
    {
        public ComplexCheckConfiguration()
        {
        }

        public ComplexCheckConfiguration(params CheckProviderConfiguration[] configurations)
        {
            Configurations = configurations;
        }

        public CheckProviderConfiguration[] Configurations { get; }

        public override IEnumerable<CheckInputParameter> CheckParameters =>
            Configurations.SelectMany(configuration => configuration.CheckParameters)
                          .GroupBy(parameter => parameter.XPath)
                          .Select(grouping =>
                              grouping.Aggregate((result, parameter) => !parameter.IsRequired ? result : parameter));
    }
}