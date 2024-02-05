using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Configurations
{
    internal class FacialSimilarityCheckConfiguration : OnfidoCheckConfiguration
    {
        // TODO: remove when all Selfie data will be backfilled
        public bool IsSelfieOptional { get; set; }

        public bool IsVideoRequired { get; set; }

        public string SelfieXPath => IsVideoRequired ? XPathes.SelfieVideo : XPathes.SelfiePhoto;

        public override IEnumerable<CheckInputParameter> CheckParameters =>
            new[]
            {
                CheckInputParameter.Required(XPathes.FullName),
                CheckInputParameter.Required(XPathes.ProofOfIdentityDocument),
                IsSelfieOptional
                    ? CheckInputParameter.Optional(SelfieXPath)
                    : CheckInputParameter.Required(SelfieXPath)
            };
    }
}