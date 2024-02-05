using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UpdateVerificationDetailsRequest
    {
        [NotRequired]
        public string[] TaxResidence { get; set; }

        [NotRequired]
        public TinDto Tin { get; set; }

        [NotRequired]
        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        [NotRequired]
        public bool? IsAdverseMedia { get; set; }

        [NotRequired]
        public bool? IsPep { get; set; }

        [NotRequired]
        public bool? IsSanctioned { get; set; }

        public string Reason { get; set; }
    }
}