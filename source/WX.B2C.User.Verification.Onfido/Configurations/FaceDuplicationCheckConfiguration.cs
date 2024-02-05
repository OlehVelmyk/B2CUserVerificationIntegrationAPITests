using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Configurations
{
    internal class FaceDuplicationCheckConfiguration : OnfidoCheckConfiguration
    {
        public bool IsVideoRequired { get; set; }

        public string SelfieXPath => IsVideoRequired ? XPathes.SelfieVideo : XPathes.SelfiePhoto;

        public override IEnumerable<CheckInputParameter> CheckParameters => new[]
        {
            CheckInputParameter.Required(XPathes.FullName),
            //Just to be sure that selfie is uploaded to onfido
            CheckInputParameter.Required(SelfieXPath)
        };
    }
}