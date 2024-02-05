using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class ExternalProfileDto
    {
        public ExternalProviderType Name { get; set; }

        public string Link { get; set; }
    }
}
