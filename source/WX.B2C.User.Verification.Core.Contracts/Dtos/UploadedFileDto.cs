using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class UploadedFileDto
    {
        public string Name { get; set; }

        public byte[] File { get; set; }

        public ExternalFileData ExternalData { get; set; }
    }

    public class ExternalFileData
    {
        public string Id { get; set; }

        public ExternalFileProviderType? Provider { get; set; }
    }
}