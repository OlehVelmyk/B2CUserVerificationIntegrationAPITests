using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public class ExternalProfileDto
    {
        public ExternalProviderType Provider { get; set; }

        public string Id { get; set; }
    }
}