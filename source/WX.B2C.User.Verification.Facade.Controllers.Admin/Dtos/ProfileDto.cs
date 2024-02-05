using System;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class ProfileDto
    {
        public Guid UserId { get; set; }

        [NotRequired]
        public PersonalDetailsDto PersonalDetails { get; set; }

        [NotRequired]
        public VerificationDetailsDto VerificationDetails { get; set; }
    }
}
