using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
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
        public string PoiIssuingCountry { get; set; }

        [NotRequired]
        public string PlaceOfBirth { get; set; }

        [NotRequired]
        public int? ComprehensiveIndex { get; set; }

        [NotRequired]
        public bool? IsIpMatched { get; set; }

        [NotRequired]
        public string ResolvedCountryCode { get; set; }
    }
}