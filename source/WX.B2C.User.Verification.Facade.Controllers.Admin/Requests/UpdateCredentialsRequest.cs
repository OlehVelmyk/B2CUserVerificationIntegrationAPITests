using Destructurama.Attributed;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UpdateCredentialsRequest
    {
        public string UserId { get; set; }

        [NotLogged]
        public string NewPassword { get; set; }

        [NotRequired]
        public bool? Propagate { get; set; } = true;
    }
}