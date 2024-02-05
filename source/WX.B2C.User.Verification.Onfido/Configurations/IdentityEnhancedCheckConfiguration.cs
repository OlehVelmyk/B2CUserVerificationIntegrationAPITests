using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Configurations
{
    internal class IdentityEnhancedCheckConfiguration : OnfidoCheckConfiguration
    {
        public override IEnumerable<CheckInputParameter> CheckParameters =>
            new[]
            {
                CheckInputParameter.Required(XPathes.FullName),
                CheckInputParameter.Required(XPathes.Birthdate),
                CheckInputParameter.Required(XPathes.ResidenceAddress)
            };
    }
}