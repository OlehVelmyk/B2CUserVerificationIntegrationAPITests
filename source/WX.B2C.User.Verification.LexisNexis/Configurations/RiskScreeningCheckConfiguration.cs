using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Configurations
{
    internal class RiskScreeningCheckConfiguration : CheckProviderConfiguration
    {
        public string[] SearchNames { get; set; }

        public override IEnumerable<CheckInputParameter> CheckParameters =>
            new[]
            {
                CheckInputParameter.Required(XPathes.FullName),
                CheckInputParameter.Required(XPathes.Birthdate),
                CheckInputParameter.Required(XPathes.ResidenceAddress),
                CheckInputParameter.Required(XPathes.Tin),
                CheckInputParameter.Optional(XPathes.Email)
            };
    }
}