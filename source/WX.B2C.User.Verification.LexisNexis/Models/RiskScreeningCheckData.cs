using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Models
{
    internal class RiskScreeningCheckData
    {
        public FullNameDto FullName { get; set; }

        public TinDto Tin { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public AddressDto Address { get; set; }
    }

    public class LexisNexisRiskScreeningOutputData : CheckOutputData
    {
        public bool IsPep { get; set; }

        public bool IsAdverseMedia { get; set; }

        public bool IsSanctioned { get; set; }

        public bool WithMatches(string mode, bool hasMatches)
        {
            _ = mode switch
            {
                Constants.BridgerSearchModes.Pep => WithPep(hasMatches),
                Constants.BridgerSearchModes.AdverseMedia => WithAdverseMedia(hasMatches),
                Constants.BridgerSearchModes.Sanction => WithSanction(hasMatches),
                Constants.BridgerSearchModes.PepFamilyMembers => WithPep(hasMatches),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported search mode.")
            };

            return hasMatches;
        }

        private LexisNexisRiskScreeningOutputData WithPep(bool value)
        {
            IsPep = value;
            return this;
        }

        private LexisNexisRiskScreeningOutputData WithAdverseMedia(bool value)
        {
            IsAdverseMedia = value;
            return this;
        }

        private LexisNexisRiskScreeningOutputData WithSanction(bool value)
        {
            IsSanctioned = value;
            return this;
        }
    }
}