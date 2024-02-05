namespace WX.B2C.User.Verification.Events.Dtos
{
    public class VerificationDetailsDto
    {
        public string Nationality { get; set; }

        public string PoiIssuingCountry { get; set; }

        public string PlaceOfBirth { get; set; }

        public bool? IsPep { get; set; }

        public bool? IsSanctioned { get; set; }

        public bool? IsAdverseMedia { get; set; }

        public int? ComprehensiveIndex { get; set; }

        public bool? IsIpMatched { get; set; }

        public string ResolvedCountryCode { get; set; }

        public TinDto Tin { get; set; }
    }
}
