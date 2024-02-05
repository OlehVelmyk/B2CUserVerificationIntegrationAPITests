using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    public class VerificationDetailsDto
    {
        [NotRequired]
        public string[] TaxResidence { get; set; }

        [NotRequired]
        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        [NotRequired]
        public TinDto Tin { get; set; }
    }
}