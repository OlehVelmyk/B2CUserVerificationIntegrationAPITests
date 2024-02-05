using System;
using System.IO;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Providers;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IBlobFileMapper
    {
        BlobFileDto Map(Guid userId, DocumentFileDto documentFileDto);

        BlobFileDto Map(FileDto fileDto);
    }
    
    public interface IFileBlobStorage
    {
        Task UploadAsync(BlobFileDto fileDto, byte[] file);

        /// <summary>
        /// Do not use it for directly in facade services.
        /// We have <see cref="IFileProvider"/> for downloading files with fallback strategy
        /// </summary>
        Task<Stream> DownloadAsync(BlobFileDto blobFileDto);

        Task DeleteAsync(BlobFileDto fileDto);

        Task<bool> ExistsAsync(Guid userId, Guid fileId, string fileName);

        Task<string> GenerateDownloadHrefAsync(BlobFileDto fileDto);
    }
}