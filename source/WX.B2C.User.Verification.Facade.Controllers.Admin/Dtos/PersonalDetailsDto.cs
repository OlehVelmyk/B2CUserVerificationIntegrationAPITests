using System;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class PersonalDetailsDto
    {
        [NotRequired]
        public string FirstName { get; set; }
        
        [NotRequired]
        public string LastName { get; set; }

        [NotRequired]
        public DateTime? DateOfBirth { get; set; }

        [NotRequired]
        public AddressDto ResidenceAddress { get; set; }

        [NotRequired]
        public string Nationality { get; set; }

        [NotRequired]
        public string Email { get; set; }
    }
}
