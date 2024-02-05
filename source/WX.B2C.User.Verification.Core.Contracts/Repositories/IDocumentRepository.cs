using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IDocumentRepository
    {
        Task SaveAsync(DocumentDto documentDto);

        Task<DocumentDto> GetAsync(Guid documentId);
    }
}