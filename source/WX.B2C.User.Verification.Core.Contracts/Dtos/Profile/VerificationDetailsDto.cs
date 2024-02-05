using System;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public sealed class VerificationDetailsDto
    {
        public Guid UserId { get; set; }

        public string IpAddress { get; set; }

        public string[] TaxResidence { get; set; }
        
        public RiskLevel? RiskLevel { get; set; }

        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        public TinDto Tin { get; set; }

        public string Nationality { get; set; }

        public bool? IsPep { get; set; }

        public bool? IsSanctioned { get; set; }

        public bool? IsAdverseMedia { get; set; }

        public decimal? Turnover { get; set; }

        public string PoiIssuingCountry { get; set; }

        public string PlaceOfBirth { get; set; }

        public int? ComprehensiveIndex { get; set; }

        public bool? IsIpMatched { get; set; }

        public string ResolvedCountryCode { get; set; }
    }
}