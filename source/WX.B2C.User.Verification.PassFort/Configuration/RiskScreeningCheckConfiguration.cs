using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.PassFort.Configuration
{
    internal sealed class RiskScreeningCheckConfiguration : CheckProviderConfiguration
    {
        public bool UseAdverseMediaFlow { get; set; }

        public override IEnumerable<CheckInputParameter> CheckParameters => new[]
        {
            CheckInputParameter.Required(XPathes.FullName),
            CheckInputParameter.Required(XPathes.VerifiedNationality),
            CheckInputParameter.Required(XPathes.Birthdate),
            CheckInputParameter.Required(XPathes.IdDocumentNumber),
            CheckInputParameter.Required(XPathes.ResidenceAddress),
        };
    }
}