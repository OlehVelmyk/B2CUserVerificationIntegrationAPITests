using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IDocumentCategoryMapper
    {
        DocumentCategoryLookupDto Map(DocumentCategory category, IEnumerable<DocumentType> types);
    }

    public class DocumentCategoryMapper : IDocumentCategoryMapper
    {
        public DocumentCategoryLookupDto Map(DocumentCategory category, IEnumerable<DocumentType> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            return new DocumentCategoryLookupDto
            {
                Name = category,
                Description = MapToDescription(category),
                Types = types.Select(MapType).ToArray()
            };

            static DocumentTypeLookupDto MapType(DocumentType type) => new() { Name = type.Value, Description = type.Description };
        }

        private static string MapToDescription(DocumentCategory category)
        {
            return category switch
            {
                DocumentCategory.ProofOfIdentity => "Proof of identity",
                DocumentCategory.ProofOfAddress => "Proof of address",
                DocumentCategory.Supporting => "Supporting",
                DocumentCategory.Taxation => "Taxation",
                DocumentCategory.ProofOfFunds => "Proof Of funds",
                DocumentCategory.Selfie => "Selfie",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}
