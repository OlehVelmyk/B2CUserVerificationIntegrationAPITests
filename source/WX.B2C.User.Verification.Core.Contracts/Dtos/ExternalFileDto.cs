using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class ExternalFileDto
    {
        public string Id { get; set; }

        public ExternalFileProviderType Provider { get; set; }

        public string DocumentType { get; set; }
    }
}