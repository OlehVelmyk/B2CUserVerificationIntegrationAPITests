using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IDocumentTypeProvider
    {
        IDictionary<DocumentCategory, IEnumerable<DocumentType>> Get();

        IEnumerable<DocumentType> Get(DocumentCategory category);

        DocumentType Get(DocumentCategory category, string type);

        bool IsValid(DocumentCategory category, string documentType);
    }
}
