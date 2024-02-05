using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class AddressDto
    {
        [NotRequired]
        public string Line1 { get; set; }

        [NotRequired]
        public string Line2 { get; set; }

        [NotRequired]
        public string City { get; set; }

        [NotRequired]
        public string State { get; set; }

        public string Country { get; set; }
        
        [NotRequired]
        public string ZipCode { get; set; }
    }
}