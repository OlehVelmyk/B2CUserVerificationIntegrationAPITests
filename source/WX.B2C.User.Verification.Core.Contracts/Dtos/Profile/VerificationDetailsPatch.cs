using System;
using Optional;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public sealed class PersonalDetailsPatch
    {
        public Option<string> FirstName { get; set; }

        public Option<string> LastName { get; set; }

        public Option<DateTime?> DateOfBirth { get; set; }

        public Option<AddressDto> ResidenceAddress { get; set; }

        public Option<string> Nationality { get; set; }

        public Option<string> Email { get; set; }
    }

    public sealed class VerificationDetailsPatch
    {
        public Option<string> IpAddress { get; set; }

        public Option<string[]> TaxResidence { get; set; }
        
        public Option<RiskLevel?> RiskLevel { get; set; }

        public Option<IdDocumentNumberDto> IdDocumentNumber { get; set; }

        public Option<TinDto> Tin { get; set; }

        public Option<string> Nationality { get; set; }

        public Option<bool?> IsPep { get; set; }

        public Option<bool?> IsSanctioned { get; set; }

        public Option<bool?> IsAdverseMedia { get; set; }

        public Option<decimal> Turnover { get; set; }

        public Option<string> PoiIssuingCountry { get; set; }

        public Option<string> PlaceOfBirth { get; set; }

        public Option<int?> ComprehensiveIndex { get; set; }

        public Option<bool?> IsIpMatched { get; set; }

        public Option<string> ResolvedCountryCode { get; set; }
    }
}