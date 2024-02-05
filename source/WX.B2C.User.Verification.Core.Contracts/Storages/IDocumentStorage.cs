using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IDocumentStorage
    {
        Task<DocumentDto> GetAsync(Guid documentId);

        Task<DocumentDto[]> FindAsync(Guid userId, DocumentCategory? category);

        Task<DocumentDto> FindLatestAsync(Guid userId, DocumentCategory category);

        Task<DocumentDto[]> FindSubmittedDocumentsAsync(Guid userId);

        Task<DocumentDto[]> FindSubmittedDocumentsAsync(Guid userId, IEnumerable<DocumentCategory> categories);

        Task<DocumentDto> FindAsync(Guid documentId, Guid userId);
    }
}