using System;
using System.IO;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Providers
{
    public interface IFileProvider
    {
        Task<Stream> DownloadAsync(FileDto fileDto);

        Task<string> GenerateDownloadHrefAsync(Guid userId, DocumentFileDto documentFileDto);

        Task<Stream> DownloadAsync(Guid userId, DocumentFileDto documentFileDto);

        Task<DownloadedFile> DownloadAsync(Guid userId, ExternalFileDto externalFile);

        Task<string> UploadAsync(Guid userId, UploadExternalFileDto uploadFile);
    }
}