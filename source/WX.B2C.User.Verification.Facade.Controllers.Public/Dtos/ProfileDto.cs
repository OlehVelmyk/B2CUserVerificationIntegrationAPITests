using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    public class ProfileDto
    {
        [NotRequired]
        public ApplicationState? ApplicationState { get; set; }

        public VerificationDetailsDto VerificationDetails { get; set; }
    }
}
