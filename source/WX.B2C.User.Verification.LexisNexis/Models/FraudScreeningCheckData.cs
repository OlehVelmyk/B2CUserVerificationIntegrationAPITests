using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Models
{
    internal class FraudScreeningCheckData
    {
        public FullNameDto FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public AddressDto Address { get; set; }

        public TinDto Tin { get; set; }
    }

    public class LexisNexisFraudScreeningOutputData : CheckOutputData
    {
        public int? ComprehensiveIndex { get; set; }

        public RiskIndicator[] RiskIndicators { get; set; }

        public class RiskIndicator
        {
            public string RiskCode { get; set; }

            public string Description { get; set; }
        }
    }
}