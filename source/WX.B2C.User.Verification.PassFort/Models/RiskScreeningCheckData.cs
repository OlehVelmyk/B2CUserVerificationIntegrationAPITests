using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.PassFort.Models
{
    internal class RiskScreeningCheckData
    {
        public string ProfileId { get; set; }

        public DateTime BirthDate { get; set; }

        public FullNameDto FullName { get; set; }

        public string Nationality { get; set; }

        public AddressDto ResidenceAddress { get; set; }
    }

    internal class RiskScreeningCheckOutputData : CheckOutputData
    {
        public bool IsPep { get; set; }

        public bool IsAdverseMedia { get; set; }

        public bool IsSanctioned { get; set; }

        public string Matches { get; set; }
    }
}
