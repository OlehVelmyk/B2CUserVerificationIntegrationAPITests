using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IFileService
    {
        Task<Guid> UploadAsync(Guid userId, UploadedFileDto uploadedFileDto);

        Task UpdateAsync(Guid userId, Guid fileId, ExternalFileData fileData);

        Task SubmitAsync(Guid userId, Guid fileId);
    }
}