using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class IpAddressCheckConfiguration : CheckProviderConfiguration
    {
        public IpAddressMatchingType MatchType { get; set; }

        public bool ExtractOnly { get; set; }

        public override IEnumerable<CheckInputParameter> CheckParameters => new[]
        {
            CheckInputParameter.Required(XPathes.IpAddress),
            CheckInputParameter.Required(XPathes.ResidenceAddress)
        };
    }
}