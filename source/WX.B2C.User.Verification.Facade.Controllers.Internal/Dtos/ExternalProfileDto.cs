using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos
{
    public class ExternalProfileDto
    {
        public ExternalProviderType Name { get; set; }

        public string Id { get; set; }
    }
}
