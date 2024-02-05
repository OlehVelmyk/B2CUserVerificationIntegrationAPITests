using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Requests
{
    public class UpdateVerificationDetailsRequest
    {
        [NotRequired]
        public string[] TaxResidence { get; set; }

        [NotRequired]
        public TinDto Tin { get; set; }
    }
}