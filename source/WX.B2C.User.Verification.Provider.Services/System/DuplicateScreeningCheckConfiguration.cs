using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    // TODO: Implement configuration options
    internal class DuplicateScreeningCheckConfiguration : CheckProviderConfiguration
    {
        public override IEnumerable<CheckInputParameter> CheckParameters => new[]
        {
            CheckInputParameter.Required(XPathes.FullName),
            CheckInputParameter.Required(XPathes.Birthdate)
        };
    }
}
