using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IFileStorage
    {
        Task<FileDto> GetAsync(Guid fileId);

        Task<FileDto> FindAsync(Guid userId, Guid fileId);

        Task<FileDto> FindAsync(Guid userId, uint crc32Checksum);

        Task<string> GetDocumentTypeAsync(Guid fileId);
    }
}
