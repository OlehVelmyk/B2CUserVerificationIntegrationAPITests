using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos
{
    public class ProfileDto
    {
        public Guid UserId { get; set; }

        public VerificationDetailsDto VerificationDetails { get; set; }
    }

    public class VerificationDetailsDto
    {
        [NotRequired]
        public string[] TaxResidence { get; set; }

        [NotRequired]
        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        [NotRequired]
        public TinDto Tin { get; set; }

        [NotRequired]
        public string VerificationIpAddress { get; set; }

        [NotRequired]
        public bool? IsPep { get; set; }

        [NotRequired]
        public bool? IsSanctioned { get; set; }

        [NotRequired]
        public bool? IsAdverseMedia { get; set; }

        [NotRequired]
        public decimal? Turnover { get; set; }

        [NotRequired]
        public string Nationality { get; set; }

        [NotRequired]
        public string PoiIssuingCountry { get; set; }

        [NotRequired]
        public string PlaceOfBirth { get; set; }

        [NotRequired]
        public int? ComprehensiveIndex { get; set; }

        [NotRequired]
        public bool? IsIpMatched { get; set; }

        [NotRequired]
        public string ResolvedCountryCode { get; set; }

        [NotRequired]
        public RiskLevel? RiskLevel { get; set; }
    }

    public class TinDto
    {
        public string Number { get; set; }

        public TinType Type { get; set; }
    }

    public class IdDocumentNumberDto
    {
        public string Number { get; set; }

        public string Type { get; set; }
    }
}
