using System.Collections.Generic;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Providers
{
    internal static class DocumentCategoryProvider
    {
        private static readonly IDictionary<DocumentCategory, IEnumerable<string>> DocumentCategoriesTypes =
            new Dictionary<DocumentCategory, IEnumerable<string>>
            {
                [DocumentCategory.ProofOfIdentity] = DocumentTypes.IdentityDocumentTypes,
                [DocumentCategory.ProofOfAddress] = DocumentTypes.AddressDocumentTypes,
                [DocumentCategory.ProofOfFunds] = DocumentTypes.ProofOfFundsTypes,
                [DocumentCategory.Selfie] = DocumentTypes.SelfieTypes,
                [DocumentCategory.Taxation] = DocumentTypes.TaxationTypes,
                [DocumentCategory.Supporting] = DocumentTypes.SupportingTypes
            };

        public static IDictionary<DocumentCategory, IEnumerable<string>> GetAll() =>
            DocumentCategoriesTypes;
    }
}
