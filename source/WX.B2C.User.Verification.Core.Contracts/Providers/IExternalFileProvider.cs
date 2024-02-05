using System.IO;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IExternalFileProvider
    {
        Task<DownloadedFile> DownloadAsync(string profileId, ExternalFileDto externalFile);

        Task<string> UploadAsync(string profileId, UploadExternalFileDto uploadFile);
    }

    public class DownloadedFile
    {
        private DownloadedFile(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public static DownloadedFile Create(string name, byte[] data) => new(name, data);

        public string Name { get; }

        public byte[] Data { get; }
    }

    public class UploadExternalFileDto
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public Stream Stream { get; set; }

        public string DocumentType { get; set; }

        public ExternalFileProviderType Provider { get; set; }
    }
}