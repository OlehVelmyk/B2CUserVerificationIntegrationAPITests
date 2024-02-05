using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IDocumentService
    {
        Task SubmitAsync(Guid userId, SubmitDocumentDto submitDocumentDto, InitiationDto initiationDto);

        Task ArchiveAsync(Guid userId, Guid documentId, InitiationDto initiationDto);
    }
}